﻿namespace DownUnder
{
    public interface IOSInterface
    {
        void CopyTextToClipBoard(string text);
        string GetTextFromClipboard();
        float CaretBlinkTime { get; }
    }
}