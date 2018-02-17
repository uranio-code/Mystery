
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



namespace My.Resources
{

	//This class was auto-generated by the StronglyTypedResourceBuilder
	//class via a tool like ResGen or Visual Studio.
	//To add or remove a member, edit your .ResX file then rerun ResGen
	//with the /str option, or rebuild your VS project.
	///<summary>
	///  A strongly-typed resource class, for looking up localized strings, etc.
	///</summary>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), System.Diagnostics.DebuggerNonUserCodeAttribute(), System.Runtime.CompilerServices.CompilerGeneratedAttribute(), Microsoft.VisualBasic.HideModuleNameAttribute()]
	static internal class Resources
	{

		private static global::System.Resources.ResourceManager resourceMan;

		private static global::System.Globalization.CultureInfo resourceCulture;

		///<summary>
		///  Returns the cached ResourceManager instance used by this class.
		///</summary>
		[System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		static internal global::System.Resources.ResourceManager ResourceManager {
			get {
				if (object.ReferenceEquals(resourceMan, null)) {
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Mystery.Resources", typeof(Resources).Assembly);
					resourceMan = temp;
				}
				return resourceMan;
			}
		}

		///<summary>
		///  Overrides the current thread's CurrentUICulture property for all
		///  resource lookups using this strongly typed resource class.
		///</summary>
		[System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		static internal global::System.Globalization.CultureInfo Culture {
			get { return resourceCulture; }
			set { resourceCulture = value; }
		}

		///<summary>
		///  Looks up a localized string similar to CREATE TABLE [dbo].[Contents](
		///	[id] [bigint] IDENTITY(1,1) NOT NULL,
		///	[guid] [nvarchar](50) NOT NULL,
		///	[content_type] [nvarchar](50) NOT NULL,
		/// CONSTRAINT [PK_Contents] PRIMARY KEY CLUSTERED 
		///(
		///	[id] ASC
		///)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
		///) ON [PRIMARY].
		///</summary>
		static internal string createContentsTable {
			get { return ResourceManager.GetString("createContentsTable", resourceCulture); }
		}

		///<summary>
		///  Looks up a localized string similar to CREATE TABLE [dbo].[ContentSearch](
		///	[id] [bigint] NOT NULL,
		///	[Search] [nvarchar](50) NULL
		///) ON [PRIMARY]
		///
		///
		///ALTER TABLE [dbo].[ContentSearch]  WITH CHECK ADD  CONSTRAINT [FK_ID_ContentSearch] FOREIGN KEY([id])
		///REFERENCES [dbo].[Contents] ([id])
		///ON DELETE CASCADE
		///
		///ALTER TABLE [dbo].[ContentSearch] CHECK CONSTRAINT [FK_ID_ContentSearch].
		///</summary>
		static internal string createSearchTable {
			get { return ResourceManager.GetString("createSearchTable", resourceCulture); }
		}

		///<summary>
		///  Looks up a localized string similar to CREATE TABLE [dbo].[SeriliazedContent](
		///[id] [bigint] NOT NULL,
		///[graph] [nvarchar](MAx) NULL
		///) ON [PRIMARY]
		///
		///ALTER TABLE [dbo].[SeriliazedContent]  WITH CHECK ADD  CONSTRAINT [FK_ID_SeriliazedContent] FOREIGN KEY([id])
		///REFERENCES [dbo].[Contents] ([id])
		///ON DELETE CASCADE
		///
		///ALTER TABLE [dbo].[SeriliazedContent] CHECK CONSTRAINT [FK_ID_SeriliazedContent].
		///</summary>
		static internal string createSerilizedTable {
			get { return ResourceManager.GetString("createSerilizedTable", resourceCulture); }
		}

		///<summary>
		///  Looks up a localized string similar to CREATE TABLE [dbo].[BarcodeGen]([id] [bigint] IDENTITY(1,1) NOT NULL,[contentId] [bigint] NULL,[code] [nvarchar](16) NOT NULL,[booking_request] [nvarchar](50) NULL,
		/// CONSTRAINT [PK_BarcodeGen] PRIMARY KEY CLUSTERED 
		///(
		///	[id] ASC
		///)
		///) ON [PRIMARY]
		///
		///
		///ALTER TABLE [dbo].[BarcodeGen]  WITH CHECK ADD  CONSTRAINT [FK_BarcodeGen_Content] FOREIGN KEY([contentId])
		///REFERENCES [dbo].[Contents] ([id])
		///ON DELETE CASCADE
		///
		///ALTER TABLE [dbo].[BarcodeGen] CHECK CONSTRAINT [FK_BarcodeGen_Content]
		///
		///create UNIQUE CL [rest of string was truncated]&quot;;.
		///</summary>
		static internal string TableBarcodeGenerationQuery {
			get { return ResourceManager.GetString("TableBarcodeGenerationQuery", resourceCulture); }
		}
	}
}

//=======================================================
//Service provided by Telerik (www.telerik.com)
//Conversion powered by NRefactory.
//Twitter: @telerik
//Facebook: facebook.com/telerik
//=======================================================
