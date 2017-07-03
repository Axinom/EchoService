using System.Diagnostics;
using System.Web;

namespace EchoService
{
    public class Echo : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.BufferOutput = false;

            context.Response.ContentType = "application/octet-stream";
            context.Response.AddHeader("Content-Length", context.Request.ContentLength.ToString());

            Trace.WriteLine($"Incoming request with body size {context.Request.ContentLength}.");

            var inputStream = context.Request.GetBufferlessInputStream();

            long totalRead = 0;
            long totalWritten = 0;

            const int bufferSize = 0x14000;

            // This copying code is just Stream.CopyTo() with added tracing.
            int numBytes;
            byte[] buffer = new byte[bufferSize];
            while ((numBytes = inputStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                totalRead += numBytes;

                Trace.WriteLine($"Read {numBytes} bytes for a total of {totalRead}.");

                context.Response.OutputStream.Write(buffer, 0, numBytes);

                totalWritten += numBytes;

                Trace.WriteLine($"Wrote {numBytes} bytes for a total of {totalWritten}.");
            }

            Trace.WriteLine("Finished request processing.");

            context.Response.Flush();
            context.Response.Close();
        }

        public bool IsReusable => false;
    }
}