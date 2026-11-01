using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace NewModCompiler.Diagnostics
{
    // Lifecycle transitions notify the renderer. Work callbacks only update the
    // explicitly bound stream's atomic counters, without terminal output.
    public sealed class BuildProgressState
    {
        sealed class StageContext
        {
            internal string Id;
            internal long Started;
        }
        sealed class StreamContext
        {
            internal string Id, Phase;
            internal long Started, Order, Assets, Xml, Reused;
        }
        readonly Func<long> elapsed;
        readonly ConcurrentDictionary<string, StreamContext> streams = new ConcurrentDictionary<string, StreamContext>(StringComparer.Ordinal);
        StageContext stage = new StageContext();
        string legacyStream;
        long streamOrder;
        int completed, total;
        bool finished;
        internal event Action<BuildProgressSnapshot, string, string> Boundary;

        public BuildProgressState(Func<long> elapsedMilliseconds) { elapsed = elapsedMilliseconds; }
        public void SetPlan(int totalStreams) { Volatile.Write(ref total,totalStreams); }
        public void StartStage(string id)
        {
            Volatile.Write(ref stage,new StageContext { Id=id, Started=elapsed() });
            Notify("stage-started",null);
        }
        public void CompleteStage(string status)
        {
            BuildProgressSnapshot value=Snapshot();
            Volatile.Write(ref stage,new StageContext());
            streams.Clear(); Volatile.Write(ref legacyStream,null);
            Notify(value,"stage-completed",status);
        }
        public void StartStream(string id)
        {
            streams[id]=new StreamContext { Id=id, Started=elapsed(), Order=Interlocked.Increment(ref streamOrder) };
            Volatile.Write(ref legacyStream,id);
            Notify("stream-started",null);
        }
        public void CompleteStream(string status) { CompleteStream(Volatile.Read(ref legacyStream),status); }
        public void CompleteStream(string streamId,string status)
        {
            StreamContext value;
            if (streamId==null || !streams.TryRemove(streamId,out value)) return;
            if (status=="succeeded") Interlocked.Increment(ref completed);
            BuildProgressSnapshot snapshot=Snapshot(value);
            if (Volatile.Read(ref legacyStream)==streamId) Volatile.Write(ref legacyStream,null);
            Notify(snapshot,"stream-completed",status);
        }
        public void SetPhase(string id) { SetPhase(Volatile.Read(ref legacyStream),id); }
        public void SetPhase(string streamId,string id)
        {
            StreamContext value;
            if (streamId==null || Volatile.Read(ref stage).Id==null || Volatile.Read(ref finished) ||
                !streams.TryGetValue(streamId,out value) || Volatile.Read(ref value.Phase)==id) return;
            Volatile.Write(ref value.Phase,id);
            Notify("phase",null);
        }
        public void RecordWork(int assetsCompleted,int xmlParsed,int documentsReused)
        { RecordWork(Volatile.Read(ref legacyStream),assetsCompleted,xmlParsed,documentsReused); }
        public void RecordWork(string streamId,int assetsCompleted,int xmlParsed,int documentsReused)
        {
            StreamContext value;
            if (streamId==null || !streams.TryGetValue(streamId,out value)) return;
            if (assetsCompleted!=0) Interlocked.Add(ref value.Assets,assetsCompleted);
            if (xmlParsed!=0) Interlocked.Add(ref value.Xml,xmlParsed);
            if (documentsReused!=0) Interlocked.Add(ref value.Reused,documentsReused);
        }
        public void Finish(string status)
        {
            Volatile.Write(ref finished,true); Notify("finished",status);
        }
        void Notify(string boundary,string status) { Notify(Snapshot(),boundary,status); }
        void Notify(BuildProgressSnapshot value,string boundary,string status)
        {
            var handler=Boundary;
            if (handler!=null) handler(value,boundary,status);
        }
        public BuildProgressSnapshot Snapshot() { return Snapshot(null); }
        BuildProgressSnapshot Snapshot(StreamContext selected)
        {
            StageContext currentStage=Volatile.Read(ref stage);
            long now=elapsed();
            BuildStreamProgressSnapshot[] active=streams.Values.OrderBy(s=>s.Order).Select(s=>StreamSnapshot(s,now)).ToArray();
            BuildStreamProgressSnapshot item=selected!=null ? StreamSnapshot(selected,now) : active.Length==1 ? active[0] : null;
            return new BuildProgressSnapshot { Stage=currentStage.Id, Stream=item==null ? null : item.Stream,
                Phase=item==null ? null : item.Phase,
                AssetsCompleted=item==null ? active.Sum(s=>s.AssetsCompleted) : item.AssetsCompleted,
                XmlParsed=item==null ? active.Sum(s=>s.XmlParsed) : item.XmlParsed,
                DocumentsReused=item==null ? active.Sum(s=>s.DocumentsReused) : item.DocumentsReused,
                ActiveStreams=active, StreamsCompleted=Volatile.Read(ref completed), StreamsTotal=Volatile.Read(ref total),
                ElapsedMilliseconds=now, StageElapsedMilliseconds=now-currentStage.Started,
                StreamElapsedMilliseconds=item==null ? 0 : item.ElapsedMilliseconds, Finished=Volatile.Read(ref finished) };
        }
        static BuildStreamProgressSnapshot StreamSnapshot(StreamContext value,long now)
        {
            return new BuildStreamProgressSnapshot { Stream=value.Id, Phase=Volatile.Read(ref value.Phase),
                AssetsCompleted=Interlocked.Read(ref value.Assets), XmlParsed=Interlocked.Read(ref value.Xml),
                DocumentsReused=Interlocked.Read(ref value.Reused), ElapsedMilliseconds=now-value.Started };
        }
    }
    public sealed class BuildStreamProgressSnapshot
    {
        public string Stream { get; internal set; }
        public string Phase { get; internal set; }
        public long AssetsCompleted { get; internal set; }
        public long XmlParsed { get; internal set; }
        public long DocumentsReused { get; internal set; }
        public long ElapsedMilliseconds { get; internal set; }
    }
    public sealed class BuildProgressSnapshot
    {
        public string Stage { get; internal set; }
        public string Stream { get; internal set; }
        public string Phase { get; internal set; }
        public long AssetsCompleted { get; internal set; }
        public long XmlParsed { get; internal set; }
        public long DocumentsReused { get; internal set; }
        public BuildStreamProgressSnapshot[] ActiveStreams { get; internal set; }
        public int StreamsCompleted { get; internal set; }
        public int StreamsTotal { get; internal set; }
        public long ElapsedMilliseconds { get; internal set; }
        public long StageElapsedMilliseconds { get; internal set; }
        public long StreamElapsedMilliseconds { get; internal set; }
        public bool Finished { get; internal set; }
    }
}
