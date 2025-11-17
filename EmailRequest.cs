using System.Collections.Generic;
using System.IO;

namespace EmailLibrary
{
    public class EmailRequest
    {
        public string ToEmail { get; set; }
        /// <summary>
        /// Email que receberá cópia
        /// </summary>
        public string CcEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string HtmlBody { get; set; }
        public List<EmailAttachment> Attachments { get; set; } = new List<EmailAttachment>();
    }

    public class EmailAttachment
    {
        public string FileName { get; set; }
        public Stream Stream { get; set; }
        public byte[] Data { get; set; }
    }
}
