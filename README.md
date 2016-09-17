﻿# Markdown Monster
### An extensible Markdown Editor, Viewer and Weblog Publisher for Windows

![](Art/MarkdownMonster.png)

### Links
* **[Markdown Monster Site](http://markdownmonster.west-wind.com)**
* **[Download Installer](http://markdownmonster.west-wind.com/download.aspx)**
* **[Create Addins with .NET](http://markdownmonster.west-wind.com/docs/_4ne0s0qoi.htm)**
* **[Markdown Monster Addin Registry](https://github.com/RickStrahl/MarkdownMonsterAddinsRegistry)** (coming soon)  
* **[Bug Reports & Feature Requests](https://github.com/rickstrahl/MarkdownMonster/issues)**
* **[Discussion Forum](http://support.west-wind.com?forum=Markdown+Monster)**
* **[Change Log](Changelog.md)**
* [![Gitter](https://badges.gitter.im/RickStrahl/MarkdownMonster.svg)](https://gitter.im/RickStrahl/MarkdownMonster/General?utm_source=share-link&utm_medium=link&utm_campaign=share-link)

Here's what Markdown Monster looks like:

![Markdown Monster Screen Shot](ScreenShot.png)

> #### Release Candidate
> This tool is currently in Release Candidate stage. We're getting close to release, but there a few small issues to take care of. Want to help? Please let us know of any issues you run into and report them in the [project issues here](https://github.com/RickStrahl/MarkdownMonster/issues) on GitHub.

### Features
Markdown Monster provides many useful features:

#### Markdown Editor
* Syntax highlighted Markdown editing
* Live Markdown HTML preview 
* Easily customizable, HTML preview templates
* Gentle toolbar support for Markdown newbies
* Inline spell checking
* Customizable editor and editor themes
* Support for many common file types
* Edited HTML files preview HTML
* Save Markdown output to HTML
* Paste HTML text as Markdown
* Copy Markdown editor selection as HTML

#### Editing Features
* Syntax colored Markdown text
* Easily select and embed images
* Easily capture screen shots and embed the images
* Embed code snippets and see highlighted syntax coloring

#### Weblog Publisher
* Create or edit Weblog posts using Markdown
* Publish your posts to your blog (MetaWebLog,Wordpress)
* Download and edit existing posts
* Very fast publish and download process
* Publish to and remember multiple blogs
* Optional Dropbox post storage

#### Extensibility
* Create Addins with .NET code
* Simple interface, easy to implement
* Access the UI, buttons, active document
* Access document and application lifecycle events
* Two useful plugins are provided:
* Screen capture addin (SnagIt and custom tool)
* Weblog publishing addin (MetaWebLog and Wordpress)

#### Non Markdown Features
* HTML file editing with live preview
* Many other file formats can also be edited:  
JSON, XML, CSS, JavaScript, Typescript, FoxPro, CSharp and more
* Open document folder or console 

### Why another Markdown Editor?
Markdown is everywhere these days, and it's becoming a favorite format for many developers, writers and documentation experts to create lots of different kinds of content in this format. Markdown is used in a lot of different places:

* Source Code documentation files (like this one)
* Weblog posts
* Product documentation
* Message Board message entry
* Application text entry for formatted text

Personally I use Markdown for my Weblog, my message board, of course on GitHub and in a number of applications that have free form text fields that allow for formatted text - for example in our Webstore product descriptions are in Markdown. 

Having an editor that gets out of your way, yet provides a few helpful features **and lets you add custom features** that make your content creation sessions more productive are important. The ability to easily publish your Markdown to any MetaWebLog API endpoint is also useful as it allows you to easily publish to blogs or applications that allow for meta data uploads.

### Markdown Monster wants to eat your Markdown!
Markdown Monster is a Markdown editor and Viewer for Windows that lets you create edit or simple preview Markdown text. It provides basic editing functionality with a few nice usability features for embedding links, code, images and screen shots. It works great, but nothing revolutionary here. You get a responsive text editor that's got you covered with Markdown syntax highlighting, an collapsible live preview, so you can see what your output looks like, inline spellchecking and a handful of optimized menu options that help you mark up your text and embed and link content into your Markdown document.

### Customizable
Most features are optional and can be turned on and off. Want to work distraction free and see no preview or spell checking hints? You can turn them off. Want a different editor or preview theme, just switch it to one of the many editor themes and preview themes available that is more comfortable to the way you like to see things. 

The editor and previewer are HTML and JavaScript based, so you can also apply any custom styling and even hook up custom JavaScript code if you want to get fancy beyond the basic configurability. The preview themes are easy to modify HTML and CSS templates, so if you need to create a custom format so it matches your application's style it's quite easy to create a custom Preview theme or simply reference an online style sheet.

### Extensible with .NET Add-ins
But the **key feature** and the main reason I built this tool, is that it is **extensible**, so that you and I can plug additional functionality into it. Markdown Monster includes an add-in model that lets you add buttons to the UI, interact with the active document and the entire UI and attach to life cycle event to get notifications of various application events like  documents opening and closing, documents being saved and the application shutting down etc..

The Add-in interface is still in flux, but you can find out more in the [online documention](http://markdownmonster.west-wind.com/docs/_4ne0rl1zf.htm). If you have ideas or suggestions  on how to make the Add-in system better, please use the Issue system to [provide feedback in GitHub Issues](https://github.com/RickStrahl/MarkdownMonster/issues). Otherwise for general discussion you can [post a message on our message board](http://support.west-wind.com?forum=Markdown+Monster).

### Provided Add-ins
Markdown Monster uses the Add-in model internally to add base features to the core editor. Specifically the Screen Capture the Weblog Publishing modules are implemented as Add-ins and demonstrate how the Add-in model works.

* **Screen Capture Addin**  
The Screen Capture add-in supports two separate capture modes: Using Techsmith's popular and super versatile [SnagIt](http://techsmith.com/snagit) Screen Capture utility (which i **highly** recommend!) or using an integrated less featured Screen Capture module that allows capturing for Windows desktop windows and objects. To capture, simply click the capture button (camera icon) and the main app minimizes and either SnagIt or the integrate screen capture tool pops up to let you select the object to capture. You can preview and edit your captures, and when finished the captured image is linked directly into content.

![SnagIt Screen Capture Add-in](SnagItCaptureAddin.png)

* **WebLog Addin**  
Writing long blog posts is one thing I do a lot of and this is one of the reasons I actually wanted an integrated solution in a Markdown editor. You can take any Markdown and turn it into a blog post by using the Weblog add-in. Click the Weblog button on the toolbar and set up your blog (MetaWebLog or WordPress), and then specify the Weblog specifics like title, abstract, tags and Web Site to publish to. You can also download existing blog posts from your blog and edit them as Markdown (with some conversion limitations) and then republish them.

![Weblog Publishing Addin](WebLogPublishingAddin.png)  
![Weblog Publishing Addin](WebLogPublishingAddin_download.png)  

### Other Add-ins - What do you want to build?
I can think of a few add-in ideas - a quick way to commit to Git and Push would be useful for documentation solutions, or Git based blogs, so you can easily persist changes to a GitHub repository. Embedding all sorts of content like reference links, AdSense links, Amazon product links, a new post template engine etc. etc.

Or maybe you have custom applications that use Markdown text and provide an API that allows you to post the Markdown (or HTML) to the server. It's easy to build a custom add-in that lets you take either the Markdown text or rendered HTML and push it to a custom REST interface in your custom application.

## Acknowledgements
This application heavily leans several third party libraries without which this tool would not have been possible. Many thanks for the producers of these libraries:

* **[Ace Editor](https://ace.c9.io)**  
Ace Editor is a power HTML based editor platform that makes it easy to plug syntax highlighted software style editing possible in a browser. Markdown Monster uses Ace Editor for the main Markdown editing experience inside of a Web browser control that interacts with the WPF application.

* **[MahApps.Metro](http://mahapps.com/)**  
This library provides the Metro style window and theming support of the top level application shell.

* **[Dragablz](https://dragablz.net/)**  
This library provides the tab control support for the editor allowing for nicely styled tab reordering and overflow. The library also supports tab tear off tabs and layout docking altough this feature is not used in Markdown Monster.

* **[CommonMark.NET](https://github.com/Knagis/CommonMark.NET)**  
The markdown parser used to render markdown in the preview editor. CommonMark.NET is fast and easy to work with and has an excellent extensibility interface.

## License
Although we provide the source in the open, Markdown Monster is licensed software &copy; West Wind Technologies, 2016.

Markdown Monster can  be downloaded and evaluated for free, but a [reasonably priced license](http://store.west-wind.com/product/MARKDOWN_MONSTER) must be purchased for continued use. Licenses are **per user**, rather than per machine, so an individual user can use Markdown Monster on as many computers they wish with their license. 

Thanks for playing fair.

## Warranty Disclaimer: No Warranty!
IN NO EVENT SHALL THE AUTHOR, OR ANY OTHER PARTY WHO MAY MODIFY AND/OR REDISTRIBUTE 
THIS PROGRAM AND DOCUMENTATION, BE LIABLE FOR ANY COMMERCIAL, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM INCLUDING, BUT NOT LIMITED TO, LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY YOU OR LOSSES SUSTAINED BY THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS, EVEN IF YOU OR OTHER PARTIES HAVE BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.

&copy; Rick Strahl, West Wind Technologies, 2016