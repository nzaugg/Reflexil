//
// SecurityDeclaration.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Copyright (c) 2008 - 2010 Jb Evain
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;

using Mono.Collections.Generic;

namespace Mono.Cecil {

	public enum SecurityAction : ushort {
		Request = 1,
		Demand = 2,
		Assert = 3,
		Deny = 4,
		PermitOnly = 5,
		LinkDemand = 6,
		InheritDemand = 7,
		RequestMinimum = 8,
		RequestOptional = 9,
		RequestRefuse = 10,
		PreJitGrant = 11,
		PreJitDeny = 12,
		NonCasDemand = 13,
		NonCasLinkDemand = 14,
		NonCasInheritance = 15
	}

	public interface ISecurityDeclarationProvider : IMetadataTokenProvider {

		bool HasSecurityDeclarations { get; }
		Collection<SecurityDeclaration> SecurityDeclarations { get; }
	}

	public sealed class SecurityAttribute : ICustomAttribute {

		TypeReference attribute_type;

		internal Collection<CustomAttributeNamedArgument> fields;
		internal Collection<CustomAttributeNamedArgument> properties;

		public TypeReference AttributeType {
			get { return attribute_type; }
			set { attribute_type = value; }
		}

		public bool HasFields {
			get { return !fields.IsNullOrEmpty (); }
		}

		public Collection<CustomAttributeNamedArgument> Fields {
			get {
				if (fields == null)
					fields = new Collection<CustomAttributeNamedArgument> ();

				return fields;
			}
		}

		public bool HasProperties {
			get { return !properties.IsNullOrEmpty (); }
		}

		public Collection<CustomAttributeNamedArgument> Properties {
			get {
				if (properties == null)
					properties = new Collection<CustomAttributeNamedArgument> ();

				return properties;
			}
		}

		public SecurityAttribute (TypeReference attributeType)
		{
			this.attribute_type = attributeType;
		}
	}

	public sealed partial class SecurityDeclaration {

		readonly internal uint signature;
		readonly ModuleDefinition module;

		internal bool resolved;
		SecurityAction action;
		internal Collection<SecurityAttribute> security_attributes;

		public SecurityAction Action {
			get { return action; }
			set { action = value; }
		}

		public bool HasSecurityAttributes {
			get {
				Resolve ();

				return !security_attributes.IsNullOrEmpty ();
			}
		}

		public Collection<SecurityAttribute> SecurityAttributes {
			get {
				Resolve ();

				if (security_attributes == null)
					return security_attributes = new Collection<SecurityAttribute> ();

				return security_attributes;
			}
		}

		internal bool HasImage {
			get { return module != null && module.HasImage; }
		}

		internal SecurityDeclaration (SecurityAction action, uint signature, ModuleDefinition module)
		{
			this.action = action;
			this.signature = signature;
			this.module = module;
		}

		public SecurityDeclaration (SecurityAction action)
		{
			this.action = action;
			this.resolved = true;
		}

		public byte [] GetBlob ()
		{
			if (!HasImage || signature == 0)
				throw new NotSupportedException ();

			return module.Read (this, (declaration, reader) => reader.ReadSecurityDeclarationBlob (declaration.signature)); ;
		}

		void Resolve ()
		{
			if (resolved || !HasImage)
				return;

			module.Read (this, (declaration, reader) => {
				reader.ReadSecurityDeclarationSignature (declaration);
				return this;
			});

			resolved = true;
		}
	}

	static partial class Mixin {

		public static bool GetHasSecurityDeclarations (
			this ISecurityDeclarationProvider self,
			ModuleDefinition module)
		{
			return module.HasImage ()
				? module.Read (self, (provider, reader) => reader.HasSecurityDeclarations (provider))
				: false;
		}

		public static Collection<SecurityDeclaration> GetSecurityDeclarations (
			this ISecurityDeclarationProvider self,
			ModuleDefinition module)
		{
			return module.HasImage ()
				? module.Read (self, (provider, reader) => reader.ReadSecurityDeclarations (provider))
				: new Collection<SecurityDeclaration> ();
		}
	}
}
