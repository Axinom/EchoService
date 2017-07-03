using System;
using System.Diagnostics;
using System.Web;

namespace EchoService
{
    public class Echo : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.BufferOutput = true;

            context.Response.ContentType = "application/octet-stream";
            context.Response.AddHeader("Content-Length", context.Request.ContentLength.ToString());

            Trace.WriteLine($"Incoming request with body size {context.Request.ContentLength}.");

            var inputStream = context.Request.GetBufferlessInputStream();

            long totalRead = 0;
            long totalWritten = 0;

            // "Adjust it until stuff starts working best" method was used.
            const int readBufferSize = 0x14000;
            const int writeBufferSize = 0x1400;

            int numBytes;
            byte[] buffer = new byte[readBufferSize];
            while ((numBytes = inputStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                totalRead += numBytes;

                Trace.WriteLine($"Read {numBytes} bytes for a total of {totalRead}.");

                for (var offset = 0; offset < numBytes; offset += writeBufferSize)
                {
                    var writeBytes = Math.Min(writeBufferSize, numBytes - offset);

                    context.Response.OutputStream.Write(buffer, offset, writeBytes);

                    totalWritten += writeBytes;

                    Trace.WriteLine($"Wrote {writeBytes} bytes for a total of {totalWritten}.");
                }
            }

            Trace.WriteLine("Finished request processing.");

            context.Response.Flush();
            context.Response.Close();
        }

        public bool IsReusable => false;
    }
}