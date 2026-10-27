using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;

namespace BinaryAssetBuilder.Core;

public class AsynchronousFileReader : IDisposable
{
    private const int QueueSize = 1;

    // A job owns its Chunk until a completed read transfers it to Current.
    // A dequeued job remains owned while EndRead is waiting or throwing.
    private sealed class ReadJob
    {
        internal Chunk Chunk;
        internal IAsyncResult Result;
        internal bool BeginAttempted, EndAttempted, Completed;
        internal ReadJob(Chunk chunk) { Chunk = chunk; }
    }

    private readonly Queue<ReadJob> _JobQueue = new Queue<ReadJob>();
    private FileStream _Stream;
    private Chunk _CurrentChunk;
    private bool _Disposed;

    // Explicit early disposal abandons the unread input; it is not validation.
    // Keep a cleanup failure without replacing a consumer exception in using.
    internal Exception CleanupFailure { get; private set; }

    public uint FileSize => _Stream == null ? 0u : (uint)_Stream.Length;
    public Chunk CurrentChunk => _CurrentChunk;

    public AsynchronousFileReader(string path)
    {
        try
        {
            _Stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 8, useAsync: true);
            for (int i = 0; i < QueueSize; i++) QueueRead(new Chunk());
        }
        catch
        {
            DisposePreservingPrimaryError();
            throw;
        }
    }

    private void QueueRead(Chunk chunk)
    {
        ReadJob job = null;
        try
        {
            job = new ReadJob(chunk);
            job.BeginAttempted = true;
            // Keep the original Data/Length evaluation and exception order.
            job.Result = _Stream.BeginRead(chunk.Data, 0, chunk.Data.Length, null, chunk);
            _JobQueue.Enqueue(job);
        }
        catch
        {
            if (job == null)
            {
                // Job allocation failed before the buffer reached BeginRead.
                try { chunk.Dispose(); } catch { chunk.AbandonBuffer(); }
            }
            else CleanupJob(job);
            throw;
        }
    }

    private int CompleteJob(ReadJob job)
    {
        job.EndAttempted = true;
        try
        {
            // APM EndRead itself waits for completion. Do not request
            // AsyncWaitHandle: that getter can allocate an additional event.
            // This does not claim the APM implementation uses no event.
            int read = _Stream.EndRead(job.Result);
            job.Completed = true;
            return read;
        }
        catch
        {
            try { job.Completed = job.Result.IsCompleted; } catch { }
            throw;
        }
    }

    private Exception CleanupJob(ReadJob job)
    {
        Exception failure = null;
        if (job.Result != null && !job.EndAttempted)
        {
            try { CompleteJob(job); } catch (Exception error) { failure = error; }
        }
        Chunk chunk = job.Chunk;
        job.Chunk = null;
        if (chunk != null)
        {
            try
            {
                if (job.Completed || !job.BeginAttempted) chunk.Dispose();
            }
            catch (Exception error) { if (failure == null) failure = error; }
            finally
            {
                // BeginRead can fail without returning an IAsyncResult. Be
                // conservative about whether its array reached the OS: never
                // pool an array whose read completion cannot be established.
                // Outstanding I/O retains its own reference until completion.
                chunk.AbandonBuffer();
            }
        }
        return failure;
    }

    public bool BeginRead()
    {
        if (_JobQueue.Count == 0 || _CurrentChunk != null || _Stream == null || _Disposed) return false;
        ReadJob job = _JobQueue.Dequeue();
        try
        {
            int read = CompleteJob(job);
            _CurrentChunk = job.Chunk;
            job.Chunk = null;
            _CurrentChunk.BytesRead = read;
            if (read != 0) return true;
            // Legacy EOF retains the disposed zero-byte CurrentChunk object.
            Exception failure = CloseCore();
            if (failure != null) ExceptionDispatchInfo.Capture(failure).Throw();
            return false;
        }
        catch
        {
            CleanupJob(job);
            DisposePreservingPrimaryError();
            throw;
        }
    }

    public void EndRead()
    {
        if (_CurrentChunk == null || _Stream == null || _Disposed) return;
        try
        {
            _CurrentChunk.BytesRead = 0;
            QueueRead(_CurrentChunk);
            _CurrentChunk = null;
        }
        catch
        {
            DisposePreservingPrimaryError();
            throw;
        }
    }

    private void DisposePreservingPrimaryError()
    {
        try { Dispose(); } catch { }
    }

    public void Dispose()
    {
        CloseCore();
    }

    private Exception CloseCore()
    {
        if (_Disposed) return null;
        _Disposed = true;
        Exception failure = null;
        // Synchronous disposal may wait for the outstanding APM operation. It
        // does not promise cancellation of the underlying OS read.
        while (_JobQueue.Count != 0)
        {
            Exception pendingFailure = CleanupJob(_JobQueue.Dequeue());
            if (failure == null) failure = pendingFailure;
        }
        if (_CurrentChunk != null)
        {
            try { _CurrentChunk.Dispose(); }
            catch (Exception error) { if (failure == null) failure = error; }
            finally { _CurrentChunk.AbandonBuffer(); }
        }
        FileStream stream = _Stream;
        _Stream = null;
        if (stream != null)
        {
            try { stream.Dispose(); }
            catch (Exception error) { if (failure == null) failure = error; }
        }
        CleanupFailure = failure;
        return failure;
    }
}
