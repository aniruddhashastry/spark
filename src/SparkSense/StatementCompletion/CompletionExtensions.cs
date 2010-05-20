using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using SparkSense.Parsing;


namespace SparkSense.StatementCompletion
{
    public static class CompletionExtensions
    {
        public static char GetInputCharacter(this uint key, Guid cmdGroup, IntPtr pvaIn)
        {
            if (cmdGroup == VSConstants.VSStd2K && key == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
                return (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
            return char.MinValue;
        }

        public static bool IsACommitCharacter(this uint key, char inputCharacter)
        {
            return key == (uint)VSConstants.VSStd2KCmdID.RETURN || key == (uint)VSConstants.VSStd2KCmdID.TAB || char.IsWhiteSpace(inputCharacter) || char.IsPunctuation(inputCharacter);
        }

        public static bool IsADeletionCharacter(this uint key)
        {
            return key == (uint)VSConstants.VSStd2KCmdID.BACKSPACE || key == (uint)VSConstants.VSStd2KCmdID.DELETE;
        }

        public static bool IsAMovementCharacter(this uint key)
        {
            return key == (uint)VSConstants.VSStd2KCmdID.LEFT || key == (uint)VSConstants.VSStd2KCmdID.RIGHT;
        }

        public static bool HasMovedOutOfIntelliSenseRange(this uint key, ITextExplorer textExplorer)
        {
            if (textExplorer == null) return true;

            ITextView textView = textExplorer.TextView;
            int currentPosition = textView.Caret.Position.BufferPosition.Position;
            ITrackingSpan completionSpan = textExplorer.GetTrackingSpan();
            ITextSnapshot currentSnapshot = completionSpan.TextBuffer.CurrentSnapshot;
            var completionCaretStartPosition = textExplorer.GetStartPosition();
            switch (key)
            {
                case (uint)VSConstants.VSStd2KCmdID.LEFT:
                    return currentPosition < completionCaretStartPosition;
                case (uint)VSConstants.VSStd2KCmdID.RIGHT:
                    return currentPosition > completionCaretStartPosition + completionSpan.GetSpan(currentSnapshot).Length;
                default:
                    return false;
            }
        }
    }
}
