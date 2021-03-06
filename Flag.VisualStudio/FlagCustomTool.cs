﻿using System.IO;

using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using System.Text;

namespace Flag.VisualStudio
{
    [ComVisible(true)]
    // TODO: Replace with your GUID
    [Guid("7C6AF83B-C209-4930-BB58-A4B56AFF51CE")]
    public class FlagCustomTool : IVsSingleFileGenerator
    {
        public int DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = ".gen.cs";  // the extension must include the leading period
            return VSConstants.S_OK; // signal successful completion
        }


        public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace,
          IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
            StringBuilder consoleOut = new StringBuilder();
            using (var consoleWriter = new StringWriter(consoleOut))
            {
                StringBuilder sb = new StringBuilder();
                var originalOut = Console.Out;
                try
                {
                    Console.SetOut(consoleWriter);
                    using (var tw = new StringWriter(sb))
                        new Flag.Compile.CSharp.TemplateCompiler(wszDefaultNamespace).Compile(Path.GetFileNameWithoutExtension(wszInputFilePath), bstrInputFileContents, tw);
                }
                catch (Exception ex)
                {
                    sb.AppendLine("/*An exception occured.");
                    sb.AppendLine(ex.ToString());
                    sb.AppendLine("*/");
                    Console.Write(ex);
                }
                finally
                {
                    Console.SetOut(originalOut);
                    var writes = consoleOut.ToString();
                    if (!string.IsNullOrWhiteSpace(writes))
                    {
                        sb.AppendLine("/*");
                        sb.AppendLine(writes);
                        sb.Append("*/");
                    }
                }
                pcbOutput = ApplyText(sb.ToString(), rgbOutputFileContents);
                return VSConstants.S_OK;
            }
        }

        private static uint ApplyText(string text, IntPtr[] rgbOutputFileContents)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            int length = bytes.Length;
            rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(length);
            Marshal.Copy(bytes, 0, rgbOutputFileContents[0], length);
            return (uint)length;
        }
    }
}