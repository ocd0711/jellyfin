﻿using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller;
using MediaBrowser.Model.Plugins;

namespace MediaBrowser.HtmlBrowser
{
    public class Plugin : BaseGenericPlugin<BasePluginConfiguration>
    {
        public override string Name
        {
            get { return "Html Library Browser"; }
        }

        public override void InitInServer()
        {
            var httpServer = Kernel.Instance.HttpServer;

            /*httpServer.Where(ctx => ctx.LocalPath.IndexOf("/browser/", StringComparison.OrdinalIgnoreCase) != -1).Subscribe(ctx =>
            {
                string localPath = ctx.LocalPath;
                string srch = "/browser/";

                int index = localPath.IndexOf(srch, StringComparison.OrdinalIgnoreCase);

                string resource = localPath.Substring(index + srch.Length);

                ctx.Respond(new EmbeddedResourceHandler(ctx, resource));

            });*/
        }
    }
}
