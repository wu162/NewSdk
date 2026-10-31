package newsdk.fla;

import java.io.IOException;
import java.util.*;

/** Splits event blocks without treating comments or string contents as braces. */
final class ScriptHandlers {
    static List<String> split(String source) throws IOException {
        List<String> result = new ArrayList<>(); int p = 0;
        while ((p = trivia(source, p)) < source.length()) {
            int start = p;
            int keyword = source.startsWith("onClipEvent", p) ? 11 : source.startsWith("on", p) ? 2 : 0;
            if (keyword == 0 || p + keyword < source.length() && Character.isJavaIdentifierPart(source.charAt(p + keyword)))
                throw new IOException("Expected on(...) or onClipEvent(...) event block");
            int braces = 0; boolean opened = false;
            while (p < source.length()) {
                char c = source.charAt(p);
                if (c == '\'' || c == '"') {
                    char quote = c; p++; boolean closed = false;
                    while (p < source.length()) {
                        c = source.charAt(p++);
                        if (c == '\\') { p++; continue; }
                        if (c == quote) { closed = true; break; }
                    }
                    if (!closed) throw new IOException("Unterminated ActionScript string");
                    continue;
                }
                if (c == '/' && p + 1 < source.length() && (source.charAt(p + 1) == '/' || source.charAt(p + 1) == '*')) {
                    p = trivia(source, p); continue;
                }
                p++;
                if (c == '{') { opened = true; braces++; }
                if (c == '}' && --braces == 0) break;
            }
            if (!opened || braces != 0) throw new IOException("Unterminated ActionScript event block");
            result.add(source.substring(start, p));
        }
        return result;
    }
    private static int trivia(String s, int p) throws IOException {
        while (p < s.length()) {
            if (Character.isWhitespace(s.charAt(p))) { p++; continue; }
            if (s.startsWith("//", p)) { int n = s.indexOf('\n', p + 2); p = n < 0 ? s.length() : n + 1; continue; }
            if (s.startsWith("/*", p)) { int n = s.indexOf("*/", p + 2); if (n < 0) throw new IOException("Unterminated comment"); p = n + 2; continue; }
            break;
        }
        return p;
    }
}
