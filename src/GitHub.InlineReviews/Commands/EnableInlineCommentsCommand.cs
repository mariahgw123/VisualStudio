﻿using System;
using System.Threading.Tasks;
using GitHub.VisualStudio;
using GitHub.Services.Vssdk.Commands;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using GitHub.Services;

namespace GitHub.InlineReviews.Commands
{
    [Export(typeof(IEnableInlineCommentsCommand))]
    public class EnableInlineCommentsCommand : VsCommand, IEnableInlineCommentsCommand
    {
        /// <summary>
        /// Gets the GUID of the group the command belongs to.
        /// </summary>
        public static readonly Guid CommandSet = Guids.CommandSetGuid;

        /// <summary>
        /// Gets the numeric identifier of the command.
        /// </summary>
        public const int CommandId = PkgCmdIDList.EnableInlineCommentsId;

        const string optionName = "TextViewHost/GlyphMargin";

        readonly Lazy<IVsTextManager> textManager;
        readonly Lazy<IVsEditorAdaptersFactoryService> editorAdapter;

        [ImportingConstructor]
        public EnableInlineCommentsCommand(
            IGitHubServiceProvider serviceProvider,
            Lazy<IVsEditorAdaptersFactoryService> editorAdapter) : base(CommandSet, CommandId)
        {
            textManager = new Lazy<IVsTextManager>(() => serviceProvider.GetService<SVsTextManager, IVsTextManager>());
            this.editorAdapter = editorAdapter;
        }

        public override Task Execute()
        {
            IVsTextView activeView = null;
            if (textManager.Value.GetActiveView(1, null, out activeView) == VSConstants.S_OK)
            {
                var wpfTextView = editorAdapter.Value.GetWpfTextView(activeView);
                var options = wpfTextView.Options;
                var enabled = options.GetOptionValue<bool>(optionName);
                options.SetOptionValue(optionName, !enabled);
            }

            return Task.CompletedTask;
        }
    }
}
