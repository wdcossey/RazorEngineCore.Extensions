![Logo](https://github.com/wdcossey/RazorEngineCore.Extensions/raw/master/Assets/razor.png)

# [RazorEngineCore](https://github.com/adoconnection/RazorEngineCore).Extensions
Extensions for [RazorEngineCore](https://github.com/adoconnection/RazorEngineCore) (ASP.NET Core 3.1.1 Razor Template Engine)

[![Build status](https://dev.azure.com/wdcossey/GitHub/_apis/build/status/RazorEngineCore-Extensions-CI?branchName=master)](https://dev.azure.com/wdcossey/GitHub/_build/latest?definitionId=1)
[![Build status](https://badgen.net/nuget/v/RazorEngineCore.Extensions?icon=nuget)](https://www.nuget.org/packages/RazorEngineCore.Extensions/)

HTML (safe) encoded output by *default* using `RazorEngineCorePageModel` or `RazorEngineCorePageModel<T>`.

---

Support for (some) `@Html` tags with custom `RazorEngineCoreHtmlWriter` (to reduce external dependencies)
```
use: @("<div>string</div>")
out: &lt;div&gt;string&lt;/div&gt;

use: @Html.Encode("<div>string</div>")
out: &amp;lt;div&amp;gt;string&amp;lt;/div&amp;gt;

use: @Html.AttributeEncode("<div>string</div>")
out: &amp;lt;div&gt;string&amp;lt;/div&gt;

use: @Html.Raw("<div>string</div>")
out: <div>string</div>
```

Using the default `RazorEngineTemplateBase` with `@Html` will result in an exception 
`The name 'Html' does not exist in the current context`.

---

### Extension Methods

For **RazorEngine**
```cs
CompileFromFile(string fileName, ...)
CompileFromFileAsync(string fileName, ...)
CompileFromStream(StreamReader streamReader, ...)
CompileFromStreamAsync(StreamReader streamReader, ...)
```

For **RazorEngineCompiledTemplate<>**

These enable direct Model usage w/o the need for the RazorEngineTemplateBase (PageModel) instance.
```cs
Run(object model = null)
RunAsync(object model = null)
Run<TModel>(TModel model = null)
RunAsync<TModel>(TModel model = null)
```

---

### Coming Soon

* Wiki.
* `@Json.Serialize(...)` tag support via a custom `RazorEngineCoreJsonWriter` (using `System.Text.Json`).
* Aditional `@Html` tags.
* Unit Tests.
* And more...

Razor image by [Freepik](http://www.freepik.com)
