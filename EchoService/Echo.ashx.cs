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

            var inputStream = context.Request.GetBufferlessInputStream();
            inputStream.CopyTo(context.Response.OutputStream);
        }

        public bool IsReusable => true;
    }
}