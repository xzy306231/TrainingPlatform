//
// metametadataType.cs.cs
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
using Altova.Types;

namespace imsmd_rootv1p2p1
{
	public class metametadataType : Altova.Node
	{
		#region Forward constructors
		public metametadataType() : base() { SetCollectionParents(); }
		public metametadataType(XmlDocument doc) : base(doc) { SetCollectionParents(); }
		public metametadataType(XmlNode node) : base(node) { SetCollectionParents(); }
		public metametadataType(Altova.Node node) : base(node) { SetCollectionParents(); }
		#endregion // Forward constructors

		public override void AdjustPrefix()
		{
			int nCount;

			nCount = DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "identifier");
			for (int i = 0; i < nCount; i++)
			{
				XmlNode DOMNode = GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "identifier", i);
				InternalAdjustPrefix(DOMNode, true);
			}

			nCount = DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "catalogentry");
			for (int i = 0; i < nCount; i++)
			{
				XmlNode DOMNode = GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "catalogentry", i);
				InternalAdjustPrefix(DOMNode, true);
				new catalogentryType(DOMNode).AdjustPrefix();
			}

			nCount = DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "contribute");
			for (int i = 0; i < nCount; i++)
			{
				XmlNode DOMNode = GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "contribute", i);
				InternalAdjustPrefix(DOMNode, true);
				new contributeType(DOMNode).AdjustPrefix();
			}

			nCount = DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "metadatascheme");
			for (int i = 0; i < nCount; i++)
			{
				XmlNode DOMNode = GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "metadatascheme", i);
				InternalAdjustPrefix(DOMNode, true);
			}

			nCount = DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "language");
			for (int i = 0; i < nCount; i++)
			{
				XmlNode DOMNode = GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "language", i);
				InternalAdjustPrefix(DOMNode, true);
			}
		}


		#region identifier accessor methods
		public int GetidentifierMinCount()
		{
			return 0;
		}

		public int identifierMinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetidentifierMaxCount()
		{
			return 1;
		}

		public int identifierMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetidentifierCount()
		{
			return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "identifier");
		}

		public int identifierCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "identifier");
			}
		}

		public bool Hasidentifier()
		{
			return HasDomChild(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "identifier");
		}

		public SchemaString GetidentifierAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "identifier", index)));
		}

		public SchemaString Getidentifier()
		{
			return GetidentifierAt(0);
		}

		public SchemaString identifier
		{
			get
			{
				return GetidentifierAt(0);
			}
		}

		public void RemoveidentifierAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "identifier", index);
		}

		public void Removeidentifier()
		{
			while (Hasidentifier())
				RemoveidentifierAt(0);
		}

		public void Addidentifier(SchemaString newValue)
		{
			AppendDomChild(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "identifier", newValue.ToString());
		}

		public void InsertidentifierAt(SchemaString newValue, int index)
		{
			InsertDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "identifier", index, newValue.ToString());
		}

		public void ReplaceidentifierAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "identifier", index, newValue.ToString());
		}
		#endregion // identifier accessor methods

		#region identifier collection
        public identifierCollection	Myidentifiers = new identifierCollection( );

        public class identifierCollection: IEnumerable
        {
            metametadataType parent;
            public metametadataType Parent
			{
				set
				{
					parent = value;
				}
			}
			public identifierEnumerator GetEnumerator() 
			{
				return new identifierEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class identifierEnumerator: IEnumerator 
        {
			int nIndex;
			metametadataType parent;
			public identifierEnumerator(metametadataType par) 
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
				return(nIndex < parent.identifierCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetidentifierAt(nIndex));
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
	
        #endregion // identifier collection

		#region catalogentry accessor methods
		public int GetcatalogentryMinCount()
		{
			return 0;
		}

		public int catalogentryMinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetcatalogentryMaxCount()
		{
			return Int32.MaxValue;
		}

		public int catalogentryMaxCount
		{
			get
			{
				return Int32.MaxValue;
			}
		}

		public int GetcatalogentryCount()
		{
			return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "catalogentry");
		}

		public int catalogentryCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "catalogentry");
			}
		}

		public bool Hascatalogentry()
		{
			return HasDomChild(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "catalogentry");
		}

		public catalogentryType GetcatalogentryAt(int index)
		{
			return new catalogentryType(GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "catalogentry", index));
		}

		public catalogentryType Getcatalogentry()
		{
			return GetcatalogentryAt(0);
		}

		public catalogentryType catalogentry
		{
			get
			{
				return GetcatalogentryAt(0);
			}
		}

		public void RemovecatalogentryAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "catalogentry", index);
		}

		public void Removecatalogentry()
		{
			while (Hascatalogentry())
				RemovecatalogentryAt(0);
		}

		public void Addcatalogentry(catalogentryType newValue)
		{
			AppendDomElement("http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "catalogentry", newValue);
		}

		public void InsertcatalogentryAt(catalogentryType newValue, int index)
		{
			InsertDomElementAt("http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "catalogentry", index, newValue);
		}

		public void ReplacecatalogentryAt(catalogentryType newValue, int index)
		{
			ReplaceDomElementAt("http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "catalogentry", index, newValue);
		}
		#endregion // catalogentry accessor methods

		#region catalogentry collection
        public catalogentryCollection	Mycatalogentrys = new catalogentryCollection( );

        public class catalogentryCollection: IEnumerable
        {
            metametadataType parent;
            public metametadataType Parent
			{
				set
				{
					parent = value;
				}
			}
			public catalogentryEnumerator GetEnumerator() 
			{
				return new catalogentryEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class catalogentryEnumerator: IEnumerator 
        {
			int nIndex;
			metametadataType parent;
			public catalogentryEnumerator(metametadataType par) 
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
				return(nIndex < parent.catalogentryCount );
			}
			public catalogentryType  Current 
			{
				get 
				{
					return(parent.GetcatalogentryAt(nIndex));
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
	
        #endregion // catalogentry collection

		#region contribute accessor methods
		public int GetcontributeMinCount()
		{
			return 0;
		}

		public int contributeMinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetcontributeMaxCount()
		{
			return Int32.MaxValue;
		}

		public int contributeMaxCount
		{
			get
			{
				return Int32.MaxValue;
			}
		}

		public int GetcontributeCount()
		{
			return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "contribute");
		}

		public int contributeCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "contribute");
			}
		}

		public bool Hascontribute()
		{
			return HasDomChild(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "contribute");
		}

		public contributeType GetcontributeAt(int index)
		{
			return new contributeType(GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "contribute", index));
		}

		public contributeType Getcontribute()
		{
			return GetcontributeAt(0);
		}

		public contributeType contribute
		{
			get
			{
				return GetcontributeAt(0);
			}
		}

		public void RemovecontributeAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "contribute", index);
		}

		public void Removecontribute()
		{
			while (Hascontribute())
				RemovecontributeAt(0);
		}

		public void Addcontribute(contributeType newValue)
		{
			AppendDomElement("http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "contribute", newValue);
		}

		public void InsertcontributeAt(contributeType newValue, int index)
		{
			InsertDomElementAt("http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "contribute", index, newValue);
		}

		public void ReplacecontributeAt(contributeType newValue, int index)
		{
			ReplaceDomElementAt("http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "contribute", index, newValue);
		}
		#endregion // contribute accessor methods

		#region contribute collection
        public contributeCollection	Mycontributes = new contributeCollection( );

        public class contributeCollection: IEnumerable
        {
            metametadataType parent;
            public metametadataType Parent
			{
				set
				{
					parent = value;
				}
			}
			public contributeEnumerator GetEnumerator() 
			{
				return new contributeEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class contributeEnumerator: IEnumerator 
        {
			int nIndex;
			metametadataType parent;
			public contributeEnumerator(metametadataType par) 
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
				return(nIndex < parent.contributeCount );
			}
			public contributeType  Current 
			{
				get 
				{
					return(parent.GetcontributeAt(nIndex));
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
	
        #endregion // contribute collection

		#region metadatascheme accessor methods
		public int GetmetadataschemeMinCount()
		{
			return 0;
		}

		public int metadataschemeMinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetmetadataschemeMaxCount()
		{
			return Int32.MaxValue;
		}

		public int metadataschemeMaxCount
		{
			get
			{
				return Int32.MaxValue;
			}
		}

		public int GetmetadataschemeCount()
		{
			return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "metadatascheme");
		}

		public int metadataschemeCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "metadatascheme");
			}
		}

		public bool Hasmetadatascheme()
		{
			return HasDomChild(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "metadatascheme");
		}

		public metadataschemeType GetmetadataschemeAt(int index)
		{
			return new metadataschemeType(GetDomNodeValue(GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "metadatascheme", index)));
		}

		public metadataschemeType Getmetadatascheme()
		{
			return GetmetadataschemeAt(0);
		}

		public metadataschemeType metadatascheme
		{
			get
			{
				return GetmetadataschemeAt(0);
			}
		}

		public void RemovemetadataschemeAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "metadatascheme", index);
		}

		public void Removemetadatascheme()
		{
			while (Hasmetadatascheme())
				RemovemetadataschemeAt(0);
		}

		public void Addmetadatascheme(metadataschemeType newValue)
		{
			AppendDomChild(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "metadatascheme", newValue.ToString());
		}

		public void InsertmetadataschemeAt(metadataschemeType newValue, int index)
		{
			InsertDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "metadatascheme", index, newValue.ToString());
		}

		public void ReplacemetadataschemeAt(metadataschemeType newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "metadatascheme", index, newValue.ToString());
		}
		#endregion // metadatascheme accessor methods

		#region metadatascheme collection
        public metadataschemeCollection	Mymetadataschemes = new metadataschemeCollection( );

        public class metadataschemeCollection: IEnumerable
        {
            metametadataType parent;
            public metametadataType Parent
			{
				set
				{
					parent = value;
				}
			}
			public metadataschemeEnumerator GetEnumerator() 
			{
				return new metadataschemeEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class metadataschemeEnumerator: IEnumerator 
        {
			int nIndex;
			metametadataType parent;
			public metadataschemeEnumerator(metametadataType par) 
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
				return(nIndex < parent.metadataschemeCount );
			}
			public metadataschemeType  Current 
			{
				get 
				{
					return(parent.GetmetadataschemeAt(nIndex));
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
	
        #endregion // metadatascheme collection

		#region language accessor methods
		public int GetlanguageMinCount()
		{
			return 0;
		}

		public int languageMinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetlanguageMaxCount()
		{
			return 1;
		}

		public int languageMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetlanguageCount()
		{
			return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "language");
		}

		public int languageCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "language");
			}
		}

		public bool Haslanguage()
		{
			return HasDomChild(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "language");
		}

		public SchemaString GetlanguageAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "language", index)));
		}

		public SchemaString Getlanguage()
		{
			return GetlanguageAt(0);
		}

		public SchemaString language
		{
			get
			{
				return GetlanguageAt(0);
			}
		}

		public void RemovelanguageAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "language", index);
		}

		public void Removelanguage()
		{
			while (Haslanguage())
				RemovelanguageAt(0);
		}

		public void Addlanguage(SchemaString newValue)
		{
			AppendDomChild(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "language", newValue.ToString());
		}

		public void InsertlanguageAt(SchemaString newValue, int index)
		{
			InsertDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "language", index, newValue.ToString());
		}

		public void ReplacelanguageAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "language", index, newValue.ToString());
		}
		#endregion // language accessor methods

		#region language collection
        public languageCollection	Mylanguages = new languageCollection( );

        public class languageCollection: IEnumerable
        {
            metametadataType parent;
            public metametadataType Parent
			{
				set
				{
					parent = value;
				}
			}
			public languageEnumerator GetEnumerator() 
			{
				return new languageEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class languageEnumerator: IEnumerator 
        {
			int nIndex;
			metametadataType parent;
			public languageEnumerator(metametadataType par) 
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
				return(nIndex < parent.languageCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetlanguageAt(nIndex));
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
	
        #endregion // language collection

        private void SetCollectionParents()
        {
            Myidentifiers.Parent = this; 
            Mycatalogentrys.Parent = this; 
            Mycontributes.Parent = this; 
            Mymetadataschemes.Parent = this; 
            Mylanguages.Parent = this; 
	}
}
}
