using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonMark.Parser
{
    /// <summary>
    /// Contains the regular expressions that are used in the parsers.
    /// </summary>
    internal static class Scanner
    {
        private const string scheme = "coap|doi|javascript|aaa|aaas|about|acap|cap|cid|crid|data|dav|dict|dns|file|ftp|geo|go|gopher|h323|http|https|iax|icap|im|imap|info|ipp|iris|iris.beep|iris.xpc|iris.xpcs|iris.lwz|ldap|mailto|mid|msrp|msrps|mtqp|mupdate|news|nfs|ni|nih|nntp|opaquelocktoken|pop|pres|rtsp|service|session|shttp|sieve|sip|sips|sms|snmp|soap.beep|soap.beeps|tag|tel|telnet|tftp|thismessage|tn3270|tip|tv|urn|vemmi|ws|wss|xcon|xcon-userid|xmlrpc.beep|xmlrpc.beeps|xmpp|z39.50r|z39.50s|adiumxtra|afp|afs|aim|apt|attachment|aw|beshare|bitcoin|bolo|callto|chrome|chrome-extension|com-eventbrite-attendee|content|cvs|dlna-playsingle|dlna-playcontainer|dtn|dvb|ed2k|facetime|feed|finger|fish|gg|git|gizmoproject|gtalk|hcp|icon|ipn|irc|irc6|ircs|itms|jar|jms|keyparc|lastfm|ldaps|magnet|maps|market|message|mms|ms-help|msnim|mumble|mvn|notes|oid|palm|paparazzi|platform|proxy|psyc|query|res|resource|rmi|rsync|rtmp|secondlife|sftp|sgn|skype|smb|soldat|spotify|ssh|steam|svn|teamspeak|things|udp|unreal|ut2004|ventrilo|view-source|webcal|wtai|wyciwyg|xfire|xri|ymsgr";
        private const string blocktagname = "(article|header|aside|hgroup|iframe|blockquote|hr|body|li|map|button|object|canvas|ol|caption|output|col|p|colgroup|pre|dd|progress|div|section|dl|table|td|dt|tbody|embed|textarea|fieldset|tfoot|figcaption|th|figure|thead|footer|footer|tr|form|ul|h1|h2|h3|h4|h5|h6|video|script|style)";

        private const string escapable = "[!\"#$%&\'\\(\\)*+,.\\/:;<=>?@\\[\\\\\\]^_`{|}~-]";
        private const string escaped_char = "\\\\" + escapable;
        private const string reg_char = "[^\\\\\\()\\x00-\\x20]";
        private const string in_parens_nosp = "[(]((" + reg_char + ")|(" + escaped_char + "))*[)]";

        private const string tagname = "[A-Za-z][A-Za-z0-9]*";
        private const string unquotedvalue = "[^\"'=<>`\\x00]+";
        private const string singlequotedvalue = "['][^'\\x00]*[']";
        private const string doublequotedvalue = "[\"][^\"\\x00]*[\"]";
        private const string attributevalue = "(" + unquotedvalue + ")|(" + singlequotedvalue + ")|(" + doublequotedvalue + ")";
        private const string attributevaluespec = @"\s*=\s*(" + attributevalue + ")";
        private const string attributename = "[a-zA-Z_:][a-zA-Z0-9:._-]*";
        private const string attribute = @"\s+" + attributename + "(" + attributevaluespec + ")?";
        private const string opentag = tagname + "(" + attribute + ")*" + @"\s*[/]?[>]";
        private const string closetag = "[/]" + tagname + @"\s*[>]";
        private const string htmlcomment = "\\!--(([^-\\x00]+)|([-][^-\\x00]+))*-->";
        private const string processinginstruction = "\\?(([^?>\\x00]+)|([?][^>\\x00]))*\\?>";
        private const string cdata = "\\!\\[CDATA\\[(([^\\]\\x00]+)|(\\][^\\]\\x00])|(\\]\\][^>\\x00]))*\\]\\]>";
        private const string declaration = "\\![A-Z]+\\s+[^>\\x00]*>";
        private const string htmltag = "(" + opentag + ")|(" + closetag + ")|(" + htmlcomment + ")|(" + processinginstruction + ")|(" + declaration + ")|(" + cdata + ")";

        private static readonly Regex autolink_uri = new Regex("(" + scheme + "):[^<>\\x00-\\x20]*>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex autolink_email = new Regex("[a-zA-Z0-9.!#$%&'\\*+/=?^_`{|}~-]+[@][a-zA-Z0-9]([a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?([.][a-zA-Z0-9]([a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*[>]", RegexOptions.Compiled);
        private static readonly Regex htmltagregex = new Regex(htmltag, RegexOptions.Compiled);
        private static readonly Regex html_block_tag1 = new Regex("</" + blocktagname + "(\\s|>)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex html_block_tag2 = new Regex("<" + blocktagname + "(\\s|[/>])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex html_block_tag3 = new Regex("<[!?]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex spacechars = new Regex(@"[\s]*", RegexOptions.Compiled);
        private static readonly Regex setext_header_line1 = new Regex("[=]+[ ]*$", RegexOptions.Compiled);
        private static readonly Regex setext_header_line2 = new Regex("[-]+[ ]*$", RegexOptions.Compiled);
        private static readonly Regex hrule1 = new Regex(@"([\*][ ]*){3,}[\s]*$", RegexOptions.Compiled);
        private static readonly Regex hrule2 = new Regex(@"([_][ ]*){3,}[\s]*$", RegexOptions.Compiled);
        private static readonly Regex hrule3 = new Regex(@"([-][ ]*){3,}[\s]*$", RegexOptions.Compiled);
        private static readonly Regex link_url1 = new Regex("[ \\n]*[<]([^<>\\n\\x00]|(" + escaped_char + ")|[\\\\])*[>]", RegexOptions.Compiled);
        private static readonly Regex link_url2 = new Regex("[ \\n]*((" + reg_char + ")+|(" + escaped_char + ")|(" + in_parens_nosp + "))*", RegexOptions.Compiled);
        private static readonly Regex link_title1 = new Regex("[\"]((" + escaped_char + ")|[^\"\\x00])*[\"]", RegexOptions.Compiled);
        private static readonly Regex link_title2 = new Regex("[']((" + escaped_char + ")|[^'\\x00])*[']", RegexOptions.Compiled);
        private static readonly Regex link_title3 = new Regex("[\\(]((" + escaped_char + ")|[^\\)\\x00])*[\\)]", RegexOptions.Compiled);
        private static readonly Regex atx_header_start = new Regex("[#]{1,6}([ ]+|[\\n])", RegexOptions.Compiled);
        private static readonly Regex entity = new Regex("[&]([#]([Xx][A-Fa-f0-9]{1,8}|[0-9]{1,8})|[A-Za-z][A-Za-z0-9]{1,31})[;]", RegexOptions.Compiled);
        private static readonly Regex open_code_fence1 = new Regex(@"([`]{3,})(?=[^`\n\x00]*$)", RegexOptions.Compiled);
        private static readonly Regex open_code_fence2 = new Regex(@"([~]{3,})(?=[^~\n\x00]*$)", RegexOptions.Compiled);
        private static readonly Regex close_code_fence = new Regex(@"([`]{3,}|[~]{3,})(?:\s*)$", RegexOptions.Compiled);

        private static int MatchRegex(string s, int pos, params Regex[] regexes)
        {
            Match m;
            foreach (var r in regexes)
            {
                m = r.Match(s, pos);
                if (m.Success && m.Index == pos)
                    return m.Length;
            }

            return 0;
        }

        /// <summary>
        /// Try to match URI autolink after first &lt;, returning number of chars matched.
        /// </summary>
        public static int scan_autolink_uri(string s, int pos)
        {
            /*!re2c
              scheme [:]([^\x00-\x20<>\\]|escaped_char)*[>]  { return (p - start); }
              .? { return 0; }
            */
            return MatchRegex(s, pos, autolink_uri);
        }

        /// <summary>
        /// Try to match email autolink after first &lt;, returning num of chars matched.
        /// </summary>
        public static int scan_autolink_email(string s, int pos)
        {
            /*!re2c
              [a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+
                [@]
                [a-zA-Z0-9]([a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?
                ([.][a-zA-Z0-9]([a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*
                [>] { return (p - start); }
              .? { return 0; }
            */
            return MatchRegex(s, pos, autolink_email);
        }

        /// <summary>
        /// Try to match an HTML tag after first &lt;, returning num of chars matched.
        /// </summary>
        public static int scan_html_tag(string s, int pos)
        {
            /*!re2c
              htmltag { return (p - start); }
              .? { return 0; }
            */
            return MatchRegex(s, pos, htmltagregex);
        }

        /// <summary>
        /// Try to match an HTML block tag including first &lt;, returning num of chars matched.
        /// </summary>
        public static int scan_html_block_tag(string s, int pos)
        {
            /*!re2c
              [<] [/] blocktagname (spacechar | [>])  { return (p - start); }
              [<] blocktagname (spacechar | [/>]) { return (p - start); }
              [<] [!?] { return (p - start); }
              .? { return 0; }
            */
            return MatchRegex(s, pos, html_block_tag1, html_block_tag2, html_block_tag3);
        }

        /// <summary>
        /// Try to match a URL in a link or reference, return number of chars matched.
        /// This may optionally be contained in &lt;..&gt;; otherwise
        /// whitespace and unbalanced right parentheses aren't allowed.
        /// Newlines aren't ever allowed.
        /// </summary>
        public static int scan_link_url(string s, int pos)
        {
            /*!re2c
              [ \n]* [<] ([^<>\n\\\x00] | escaped_char | [\\])* [>] { return (p - start); }
              [ \n]* (reg_char+ | escaped_char | in_parens_nosp)* { return (p - start); }
              .? { return 0; }
            */
            return MatchRegex(s, pos, link_url1, link_url2);
        }

        /// <summary>
        /// Try to match a link title (in single quotes, in double quotes, or
        /// in parentheses), returning number of chars matched.  Allow one
        /// level of internal nesting (quotes within quotes).
        /// </summary>
        public static int scan_link_title(string s, int pos)
        {
            /*!re2c
              ["] (escaped_char|[^"\x00])* ["]   { return (p - start); }
              ['] (escaped_char|[^'\x00])* ['] { return (p - start); }
              [(] (escaped_char|[^)\x00])* [)]  { return (p - start); }
              .? { return 0; }
            */
            return MatchRegex(s, pos, link_title1, link_title2, link_title3);
        }

        /// <summary>
        /// Match space characters, including newlines.
        /// </summary>
        public static int scan_spacechars(string s, int pos)
        {
            /*!re2c
              [ \t\n]* { return (p - start); }
              . { return 0; }
            */
            return MatchRegex(s, pos, spacechars);
        }
        
        /// <summary>
        /// Match ATX header start.
        /// </summary>
        public static int scan_atx_header_start(string s, int pos)
        {
            /*!re2c
              [#]{1,6} ([ ]+|[\n])  { return (p - start); }
              .? { return 0; }
            */
            return MatchRegex(s, pos, atx_header_start);
        }

        /// <summary>
        /// Match sexext header line.  Return 1 for level-1 header,
        /// 2 for level-2, 0 for no match.
        /// </summary>
        public static int scan_setext_header_line(string s, int pos)
        {
            /*!re2c
              [=]+ [ ]* [\n] { return 1; }
              [-]+ [ ]* [\n] { return 2; }
              .? { return 0; }
            */
            if (MatchRegex(s, pos, setext_header_line1) > 0)
                return 1;

            if (MatchRegex(s, pos, setext_header_line2) > 0)
                return 2;

            return 0;
        }

        /// <summary>
        /// Scan a horizontal rule line: "...three or more hyphens, asterisks,
        /// or underscores on a line by themselves. If you wish, you may use
        /// spaces between the hyphens or asterisks."
        /// </summary>
        public static int scan_hrule(string s, int pos)
        {
            return MatchRegex(s, pos, hrule1, hrule2, hrule3);
        }

        /// <summary>
        /// Scan an opening code fence.
        /// </summary>
        public static int scan_open_code_fence(string s, int pos)
        {
            /*!re2c
              [`]{3,} / [^`\n\x00]*[\n] { return (p - start); }
              [~]{3,} / [^~\n\x00]*[\n] { return (p - start); }
              .?                        { return 0; }
            */

            return MatchRegex(s, pos, open_code_fence1, open_code_fence2);
        }

        /// <summary>
        /// Scan a closing code fence with length at least len.
        /// </summary>
        public static int scan_close_code_fence(string s, int pos, int len)
        {
            /*!re2c
              ([`]{3,} | [~]{3,}) / spacechar* [\n]
                                          { if (p - start > len) {
                                            return (p - start);
                                          } else {
                                            return 0;
                                          } }
              .? { return 0; }
            */
            var p = MatchRegex(s, pos, close_code_fence);
            if (p > len)
                return p;

            return 0;
        }

        /// <summary>
        /// Scans an entity.
        /// Returns number of chars matched.
        /// </summary>
        public static int scan_entity(string s, int pos)
        {
            /*!re2c
              [&] ([#] ([Xx][A-Fa-f0-9]{1,8}|[0-9]{1,8}) |[A-Za-z][A-Za-z0-9]{1,31} ) [;]
                 { return (p - start); }
              .? { return 0; }
            */
            return MatchRegex(s, pos, entity);
        }
    }
}
