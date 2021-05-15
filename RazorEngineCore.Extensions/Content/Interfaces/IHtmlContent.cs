using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace RazorEngineCore
{
    public interface IHtmlContent
    {
        /// <summary>
        /// Writes the content by encoding it with the specified <paramref name="encoder"/>
        /// to the specified <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> to which the content is written.</param>
        /// <param name="encoder">The <see cref="HtmlEncoder"/> which encodes the content to be written.</param>
        void WriteTo(TextWriter writer, HtmlEncoder encoder);
        
        /// <summary>
        /// Writes the content by encoding it with the specified <paramref name="encoder"/>
        /// to the specified <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> to which the content is written.</param>
        /// <param name="encoder">The <see cref="HtmlEncoder"/> which encodes the content to be written.</param>
        Task WriteToAsync(TextWriter writer, HtmlEncoder encoder);
    }
}