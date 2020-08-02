//
// langstringType.cs.cs
//
// This file was generated by XMLSPY 2004 Enterprise Edition.
//
// YOU SHOULD NOT MODIFY THIS FILE, BECAUSE IT WILL BE
// OVERWRITTEN WHEN YOU RE-RUN CODE GENERATION.
//
// Refer to the XMLSPY Documentation for further details.
// http://www.altova.com/xmlspy
//


using System;
using System.Collections;
using System.Xml;
using adlcp_v1p3.Altova.Types;

namespace adlcp_v1p3.imsmd_rootv1p2p1
{
	public class langstringType : Altova.Node
	{
		#region Forward constructors
		public langstringType() : base() { SetCollectionParents(); }
		public langstringType(XmlDocument doc) : base(doc) { SetCollectionParents(); }
		public langstringType(XmlNode node) : base(node) { SetCollectionParents(); }
		public langstringType(Altova.Node node) : base(node) { SetCollectionParents(); }
		#endregion // Forward constructors

		#region Value accessor methods
		public SchemaString GetValue()
		{
			return new SchemaString(GetDomNodeValue(domNode));
		}

		public void setValue(SchemaString newValue)
		{
			SetDomNodeValue(domNode, newValue.ToString());
		}

		public SchemaString Value
		{
			get
			{
				return new SchemaString(GetDomNodeValue(domNode));
			}
			set
			{
				SetDomNodeValue(domNode, value.ToString());
			}
		}
		#endregion // Value accessor methods

		public override void AdjustPrefix()
		{
			int nCount;

			nCount = DomChildCount(NodeType.Attribute, "http://www.w3.org/XML/1998/namespace", "lang");
			for (int i = 0; i < nCount; i++)
			{
				XmlNode DOMNode = GetDomChildAt(NodeType.Attribute, "http://www.w3.org/XML/1998/namespace", "lang", i);
				InternalAdjustPrefix(DOMNode, true);
			}
		}


		#region lang accessor methods
		public int GetlangMinCount()
		{
			return 0;
		}

		public int langMinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetlangMaxCount()
		{
			return 1;
		}

		public int langMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetlangCount()
		{
			return DomChildCount(NodeType.Attribute, "http://www.w3.org/XML/1998/namespace", "lang");
		}

		public int langCount
		{
			get
			{
				return DomChildCount(NodeType.Attribute, "http://www.w3.org/XML/1998/namespace", "lang");
			}
		}

		public bool Haslang()
		{
			return HasDomChild(NodeType.Attribute, "http://www.w3.org/XML/1998/namespace", "lang");
		}

		public SchemaString GetlangAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Attribute, "http://www.w3.org/XML/1998/namespace", "lang", index)));
		}

		public SchemaString Getlang()
		{
			return GetlangAt(0);
		}

		public SchemaString lang
		{
			get
			{
				return GetlangAt(0);
			}
		}

		public void RemovelangAt(int index)
		{
			RemoveDomChildAt(NodeType.Attribute, "http://www.w3.org/XML/1998/namespace", "lang", index);
		}

		public void Removelang()
		{
			while (Haslang())
				RemovelangAt(0);
		}

		public void Addlang(SchemaString newValue)
		{
			AppendDomChild(NodeType.Attribute, "http://www.w3.org/XML/1998/namespace", "lang", newValue.ToString());
		}

		public void InsertlangAt(SchemaString newValue, int index)
		{
			InsertDomChildAt(NodeType.Attribute, "http://www.w3.org/XML/1998/namespace", "lang", index, newValue.ToString());
		}

		public void ReplacelangAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Attribute, "http://www.w3.org/XML/1998/namespace", "lang", index, newValue.ToString());
		}
		#endregion // lang accessor methods

		#region lang collection
        public langCollection	Mylangs = new langCollection( );

        public class langCollection: IEnumerable
        {
            langstringType parent;
            public langstringType Parent
			{
				set
				{
					parent = value;
				}
			}
			public langEnumerator GetEnumerator() 
			{
				return new langEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class langEnumerator: IEnumerator 
        {
			int nIndex;
			langstringType parent;
			public langEnumerator(langstringType par) 
			{
				parent = par;
				nIndex = -1;
			}
			public void Reset() 
			{
				nIndex = -1;
			}
			public bool MoveNext() 
			{
				nIndex++;
				return(nIndex < parent.langCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetlangAt(nIndex));
				}
			}
			object IEnumerator.Current 
			{
				get 
				{
					return(Current);
				}
			}
    	}
	
        #endregion // lang collection

        private void SetCollectionParents()
        {
            Mylangs.Parent = this; 
	}
}
}
