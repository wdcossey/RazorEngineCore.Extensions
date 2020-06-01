﻿using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

namespace RazorEngineCore
{
    public abstract class RazorEngineCorePageModel : RazorEngineTemplateBase, IRazorEngineTemplate
    {
        private readonly TextWriter output = new StringWriter();

        private HtmlEncoder htmlEncoder => HtmlEncoder.Default;
        private AttributeInfo _attributeInfo;
        
        private string attributePrefix = null;
        private string attributeSuffix = null;
        
        public RazorEngineCoreHtmlWriter Html => new RazorEngineCoreHtmlWriter();

        public virtual void WriteLiteral(object value)
        {
            
            if (value == null)
            {
                return;
            }

            WriteLiteral(value.ToString());
        }
        
        public new virtual void WriteLiteral(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                output.Write(value);
            }
        }
        
        public virtual void Write(string value)
        {
            var writer = output;
            var encoder = htmlEncoder;
            if (!string.IsNullOrEmpty(value))
            {
                // Perf: Encode right away instead of writing it character-by-character.
                // character-by-character isn't efficient when using a writer backed by a ViewBuffer.
                var encoded = encoder.Encode(value);
                writer.Write(encoded);
            }
        }

        public override void Write(object value = null)
        {
            if (value == null || value == HtmlString.Empty)
            {
                return;
            }

            var writer = output;
            var encoder = htmlEncoder;
            if (value is IHtmlContent htmlContent)
            {
                var bufferedWriter = writer as ViewBufferTextWriter;
                if (bufferedWriter == null || !bufferedWriter.IsBuffering)
                {
                    htmlContent.WriteTo(writer, encoder);
                }
                else
                {
                    if (value is IHtmlContentContainer htmlContentContainer)
                    {
                        // This is likely another ViewBuffer.
                        htmlContentContainer.MoveTo(bufferedWriter.Buffer);
                    }
                    else
                    {
                        // Perf: This is the common case for IHtmlContent, ViewBufferTextWriter is inefficient
                        // for writing character by character.
                        // ReSharper disable once MustUseReturnValue
                        bufferedWriter.Buffer.AppendHtml(htmlContent);
                    }
                }

                return;
            }

            Write(value.ToString());
        }
        
        public override void BeginWriteAttribute(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException(nameof(prefix));
            }

            if (suffix == null)
            {
                throw new ArgumentNullException(nameof(suffix));
            }

            _attributeInfo = new AttributeInfo(name, prefix, prefixOffset, suffix, suffixOffset, attributeValuesCount);

            // Single valued attributes might be omitted in entirety if it the attribute value strictly evaluates to
            // null  or false. Consequently defer the prefix generation until we encounter the attribute value.
            if (attributeValuesCount != 1)
            {
                WritePositionTaggedLiteral(prefix, prefixOffset);
            }
            
            //this.attributeSuffix = suffix;
            //this.stringBuilder.Append(prefix);
            //this.stringBuilder.Append(this.attributePrefix);
        }

        public override void WriteAttributeValue(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral)
        {
            if (_attributeInfo.AttributeValuesCount == 1)
            {
                if (IsBoolFalseOrNullValue(prefix, value))
                {
                    // Value is either null or the bool 'false' with no prefix; don't render the attribute.
                    _attributeInfo.Suppressed = true;
                    return;
                }

                // We are not omitting the attribute. Write the prefix.
                WritePositionTaggedLiteral(_attributeInfo.Prefix, _attributeInfo.PrefixOffset);

                if (IsBoolTrueWithEmptyPrefixValue(prefix, value))
                {
                    // The value is just the bool 'true', write the attribute name instead of the string 'True'.
                    value = _attributeInfo.Name;
                }
            }

            // This block handles two cases.
            // 1. Single value with prefix.
            // 2. Multiple values with or without prefix.
            if (value != null)
            {
                if (!string.IsNullOrEmpty(prefix))
                {
                    WritePositionTaggedLiteral(prefix, prefixOffset);
                }

                BeginContext(valueOffset, valueLength, isLiteral);

                WriteUnprefixedAttributeValue(value, isLiteral);

                EndContext();
            }
            
            //this.stringBuilder.Append(prefix);
            //this.stringBuilder.Append(value);
        }

        public new void EndWriteAttribute()
        {
            //this.stringBuilder.Append(this.attributeSuffix);
            //this.attributeSuffix = null;
        }

        public override Task ExecuteAsync()
        {
            return base.ExecuteAsync();
        }

        public new string Result()
        {
            return this.output.ToString();
        }
        
        private void WritePositionTaggedLiteral(string value, int position)
        {
            BeginContext(position, value.Length, isLiteral: true);
            WriteLiteral(value);
            EndContext();
        }

        public virtual void BeginContext(int position, int length, bool isLiteral)
        {
            
        }

        public virtual void EndContext()
        {
            
        }
        
        private bool IsBoolFalseOrNullValue(string prefix, object value)
        {
            return string.IsNullOrEmpty(prefix) &&
                   (value == null ||
                    (value is bool && !(bool)value));
        }

        private bool IsBoolTrueWithEmptyPrefixValue(string prefix, object value)
        {
            // If the value is just the bool 'true', use the attribute name as the value.
            return string.IsNullOrEmpty(prefix) &&
                   (value is bool && (bool)value);
        }

        public virtual void EnsureRenderedBodyOrSections()
        {
            /*// a) all sections defined for this page are rendered.
            // b) if no sections are defined, then the body is rendered if it's available.
            if (PreviousSectionWriters != null && PreviousSectionWriters.Count > 0)
            {
                var sectionsNotRendered = PreviousSectionWriters.Keys.Except(
                    _renderedSections,
                    StringComparer.OrdinalIgnoreCase);

                string[] sectionsNotIgnored;
                if (_ignoredSections != null)
                {
                    sectionsNotIgnored = sectionsNotRendered.Except(_ignoredSections, StringComparer.OrdinalIgnoreCase).ToArray();
                }
                else
                {
                    sectionsNotIgnored = sectionsNotRendered.ToArray();
                }

                if (sectionsNotIgnored.Length > 0)
                {
                    var sectionNames = string.Join(", ", sectionsNotIgnored);
                    throw new InvalidOperationException(Resources.FormatSectionsNotRendered(Path, sectionNames, nameof(IgnoreSection)));
                }
            }
            else if (BodyContent != null && !_renderedBody && !_ignoreBody)
            {
                // There are no sections defined, but RenderBody was NOT called.
                // If a body was defined and the body not ignored, then RenderBody should have been called.
                var message = Resources.FormatRenderBodyNotCalled(nameof(RenderBody), Path, nameof(IgnoreBody));
                throw new InvalidOperationException(message);
            }*/
        }
        
        private void WriteUnprefixedAttributeValue(object value, bool isLiteral)
        {
            var stringValue = value as string;

            // The extra branching here is to ensure that we call the Write*To(string) overload where possible.
            if (isLiteral && stringValue != null)
            {
                WriteLiteral(stringValue);
            }
            else if (isLiteral)
            {
                WriteLiteral(value);
            }
            else if (stringValue != null)
            {
                Write(stringValue);
            }
            else
            {
                Write(value);
            }
        }
        
        private struct AttributeInfo
        {
            public AttributeInfo(
                string name,
                string prefix,
                int prefixOffset,
                string suffix,
                int suffixOffset,
                int attributeValuesCount)
            {
                Name = name;
                Prefix = prefix;
                PrefixOffset = prefixOffset;
                Suffix = suffix;
                SuffixOffset = suffixOffset;
                AttributeValuesCount = attributeValuesCount;

                Suppressed = false;
            }

            public int AttributeValuesCount { get; }

            public string Name { get; }

            public string Prefix { get; }

            public int PrefixOffset { get; }

            public string Suffix { get; }

            public int SuffixOffset { get; }

            public bool Suppressed { get; set; }
        }
    }
}