//
// resourceType.cs.cs
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
	public class resourceType : Altova.Node
	{
		#region Forward constructors
		public resourceType() : base() { SetCollectionParents(); }
		public resourceType(XmlDocument doc) : base(doc) { SetCollectionParents(); }
		public resourceType(XmlNode node) : base(node) { SetCollectionParents(); }
		public resourceType(Altova.Node node) : base(node) { SetCollectionParents(); }
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

			nCount = DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "description");
			for (int i = 0; i < nCount; i++)
			{
				XmlNode DOMNode = GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "description", i);
				InternalAdjustPrefix(DOMNode, true);
				new descriptionType(DOMNode).AdjustPrefix();
			}

			nCount = DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "catalogentry");
			for (int i = 0; i < nCount; i++)
			{
				XmlNode DOMNode = GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "catalogentry", i);
				InternalAdjustPrefix(DOMNode, true);
				new catalogentryType(DOMNode).AdjustPrefix();
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
            resourceType parent;
            public resourceType Parent
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
			resourceType parent;
			public identifierEnumerator(resourceType par) 
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

		#region description accessor methods
		public int GetdescriptionMinCount()
		{
			return 0;
		}

		public int descriptionMinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetdescriptionMaxCount()
		{
			return 1;
		}

		public int descriptionMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetdescriptionCount()
		{
			return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "description");
		}

		public int descriptionCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "description");
			}
		}

		public bool Hasdescription()
		{
			return HasDomChild(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "description");
		}

		public descriptionType GetdescriptionAt(int index)
		{
			return new descriptionType(GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "description", index));
		}

		public descriptionType Getdescription()
		{
			return GetdescriptionAt(0);
		}

		public descriptionType description
		{
			get
			{
				return GetdescriptionAt(0);
			}
		}

		public void RemovedescriptionAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "description", index);
		}

		public void Removedescription()
		{
			while (Hasdescription())
				RemovedescriptionAt(0);
		}

		public void Adddescription(descriptionType newValue)
		{
			AppendDomElement("http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "description", newValue);
		}

		public void InsertdescriptionAt(descriptionType newValue, int index)
		{
			InsertDomElementAt("http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "description", index, newValue);
		}

		public void ReplacedescriptionAt(descriptionType newValue, int index)
		{
			ReplaceDomElementAt("http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "description", index, newValue);
		}
		#endregion // description accessor methods

		#region description collection
        public descriptionCollection	Mydescriptions = new descriptionCollection( );

        public class descriptionCollection: IEnumerable
        {
            resourceType parent;
            public resourceType Parent
			{
				set
				{
					parent = value;
				}
			}
			public descriptionEnumerator GetEnumerator() 
			{
				return new descriptionEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class descriptionEnumerator: IEnumerator 
        {
			int nIndex;
			resourceType parent;
			public descriptionEnumerator(resourceType par) 
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
				return(nIndex < parent.descriptionCount );
			}
			public descriptionType  Current 
			{
				get 
				{
					return(parent.GetdescriptionAt(nIndex));
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
	
        #endregion // description collection

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
            resourceType parent;
            public resourceType Parent
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
			resourceType parent;
			public catalogentryEnumerator(resourceType par) 
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

        private void SetCollectionParents()
        {
            Myidentifiers.Parent = this; 
            Mydescriptions.Parent = this; 
            Mycatalogentrys.Parent = this; 
	}
}
}
