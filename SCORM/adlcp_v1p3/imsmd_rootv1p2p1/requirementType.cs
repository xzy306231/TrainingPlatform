//
// requirementType.cs.cs
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
	public class requirementType : Altova.Node
	{
		#region Forward constructors
		public requirementType() : base() { SetCollectionParents(); }
		public requirementType(XmlDocument doc) : base(doc) { SetCollectionParents(); }
		public requirementType(XmlNode node) : base(node) { SetCollectionParents(); }
		public requirementType(Altova.Node node) : base(node) { SetCollectionParents(); }
		#endregion // Forward constructors

		public override void AdjustPrefix()
		{
			int nCount;

			nCount = DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "type");
			for (int i = 0; i < nCount; i++)
			{
				XmlNode DOMNode = GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "type", i);
				InternalAdjustPrefix(DOMNode, true);
				new typeType2(DOMNode).AdjustPrefix();
			}

			nCount = DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "name");
			for (int i = 0; i < nCount; i++)
			{
				XmlNode DOMNode = GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "name", i);
				InternalAdjustPrefix(DOMNode, true);
				new nameType(DOMNode).AdjustPrefix();
			}

			nCount = DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "minimumversion");
			for (int i = 0; i < nCount; i++)
			{
				XmlNode DOMNode = GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "minimumversion", i);
				InternalAdjustPrefix(DOMNode, true);
			}

			nCount = DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "maximumversion");
			for (int i = 0; i < nCount; i++)
			{
				XmlNode DOMNode = GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "maximumversion", i);
				InternalAdjustPrefix(DOMNode, true);
			}
		}


		#region type2 accessor methods
		public int Gettype2MinCount()
		{
			return 0;
		}

		public int type2MinCount
		{
			get
			{
				return 0;
			}
		}

		public int Gettype2MaxCount()
		{
			return 1;
		}

		public int type2MaxCount
		{
			get
			{
				return 1;
			}
		}

		public int Gettype2Count()
		{
			return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "type");
		}

		public int type2Count
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "type");
			}
		}

		public bool Hastype2()
		{
			return HasDomChild(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "type");
		}

		public typeType2 Gettype2At(int index)
		{
			return new typeType2(GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "type", index));
		}

		public typeType2 Gettype2()
		{
			return Gettype2At(0);
		}

		public typeType2 type2
		{
			get
			{
				return Gettype2At(0);
			}
		}

		public void Removetype2At(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "type", index);
		}

		public void Removetype2()
		{
			while (Hastype2())
				Removetype2At(0);
		}

		public void Addtype2(typeType2 newValue)
		{
			AppendDomElement("http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "type", newValue);
		}

		public void Inserttype2At(typeType2 newValue, int index)
		{
			InsertDomElementAt("http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "type", index, newValue);
		}

		public void Replacetype2At(typeType2 newValue, int index)
		{
			ReplaceDomElementAt("http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "type", index, newValue);
		}
		#endregion // type2 accessor methods

		#region type2 collection
        public type2Collection	Mytype2s = new type2Collection( );

        public class type2Collection: IEnumerable
        {
            requirementType parent;
            public requirementType Parent
			{
				set
				{
					parent = value;
				}
			}
			public type2Enumerator GetEnumerator() 
			{
				return new type2Enumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class type2Enumerator: IEnumerator 
        {
			int nIndex;
			requirementType parent;
			public type2Enumerator(requirementType par) 
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
				return(nIndex < parent.type2Count );
			}
			public typeType2  Current 
			{
				get 
				{
					return(parent.Gettype2At(nIndex));
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
	
        #endregion // type2 collection

		#region name accessor methods
		public int GetnameMinCount()
		{
			return 0;
		}

		public int nameMinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetnameMaxCount()
		{
			return 1;
		}

		public int nameMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetnameCount()
		{
			return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "name");
		}

		public int nameCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "name");
			}
		}

		public bool Hasname()
		{
			return HasDomChild(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "name");
		}

		public nameType GetnameAt(int index)
		{
			return new nameType(GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "name", index));
		}

		public nameType Getname()
		{
			return GetnameAt(0);
		}

		public nameType name
		{
			get
			{
				return GetnameAt(0);
			}
		}

		public void RemovenameAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "name", index);
		}

		public void Removename()
		{
			while (Hasname())
				RemovenameAt(0);
		}

		public void Addname(nameType newValue)
		{
			AppendDomElement("http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "name", newValue);
		}

		public void InsertnameAt(nameType newValue, int index)
		{
			InsertDomElementAt("http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "name", index, newValue);
		}

		public void ReplacenameAt(nameType newValue, int index)
		{
			ReplaceDomElementAt("http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "name", index, newValue);
		}
		#endregion // name accessor methods

		#region name collection
        public nameCollection	Mynames = new nameCollection( );

        public class nameCollection: IEnumerable
        {
            requirementType parent;
            public requirementType Parent
			{
				set
				{
					parent = value;
				}
			}
			public nameEnumerator GetEnumerator() 
			{
				return new nameEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class nameEnumerator: IEnumerator 
        {
			int nIndex;
			requirementType parent;
			public nameEnumerator(requirementType par) 
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
				return(nIndex < parent.nameCount );
			}
			public nameType  Current 
			{
				get 
				{
					return(parent.GetnameAt(nIndex));
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
	
        #endregion // name collection

		#region minimumversion accessor methods
		public int GetminimumversionMinCount()
		{
			return 0;
		}

		public int minimumversionMinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetminimumversionMaxCount()
		{
			return 1;
		}

		public int minimumversionMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetminimumversionCount()
		{
			return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "minimumversion");
		}

		public int minimumversionCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "minimumversion");
			}
		}

		public bool Hasminimumversion()
		{
			return HasDomChild(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "minimumversion");
		}

		public maximumversionType GetminimumversionAt(int index)
		{
			return new maximumversionType(GetDomNodeValue(GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "minimumversion", index)));
		}

		public maximumversionType Getminimumversion()
		{
			return GetminimumversionAt(0);
		}

		public maximumversionType minimumversion
		{
			get
			{
				return GetminimumversionAt(0);
			}
		}

		public void RemoveminimumversionAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "minimumversion", index);
		}

		public void Removeminimumversion()
		{
			while (Hasminimumversion())
				RemoveminimumversionAt(0);
		}

		public void Addminimumversion(maximumversionType newValue)
		{
			AppendDomChild(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "minimumversion", newValue.ToString());
		}

		public void InsertminimumversionAt(maximumversionType newValue, int index)
		{
			InsertDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "minimumversion", index, newValue.ToString());
		}

		public void ReplaceminimumversionAt(maximumversionType newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "minimumversion", index, newValue.ToString());
		}
		#endregion // minimumversion accessor methods

		#region minimumversion collection
        public minimumversionCollection	Myminimumversions = new minimumversionCollection( );

        public class minimumversionCollection: IEnumerable
        {
            requirementType parent;
            public requirementType Parent
			{
				set
				{
					parent = value;
				}
			}
			public minimumversionEnumerator GetEnumerator() 
			{
				return new minimumversionEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class minimumversionEnumerator: IEnumerator 
        {
			int nIndex;
			requirementType parent;
			public minimumversionEnumerator(requirementType par) 
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
				return(nIndex < parent.minimumversionCount );
			}
			public maximumversionType  Current 
			{
				get 
				{
					return(parent.GetminimumversionAt(nIndex));
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
	
        #endregion // minimumversion collection

		#region maximumversion accessor methods
		public int GetmaximumversionMinCount()
		{
			return 0;
		}

		public int maximumversionMinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetmaximumversionMaxCount()
		{
			return 1;
		}

		public int maximumversionMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetmaximumversionCount()
		{
			return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "maximumversion");
		}

		public int maximumversionCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "maximumversion");
			}
		}

		public bool Hasmaximumversion()
		{
			return HasDomChild(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "maximumversion");
		}

		public minimumversionType GetmaximumversionAt(int index)
		{
			return new minimumversionType(GetDomNodeValue(GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "maximumversion", index)));
		}

		public minimumversionType Getmaximumversion()
		{
			return GetmaximumversionAt(0);
		}

		public minimumversionType maximumversion
		{
			get
			{
				return GetmaximumversionAt(0);
			}
		}

		public void RemovemaximumversionAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "maximumversion", index);
		}

		public void Removemaximumversion()
		{
			while (Hasmaximumversion())
				RemovemaximumversionAt(0);
		}

		public void Addmaximumversion(minimumversionType newValue)
		{
			AppendDomChild(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "maximumversion", newValue.ToString());
		}

		public void InsertmaximumversionAt(minimumversionType newValue, int index)
		{
			InsertDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "maximumversion", index, newValue.ToString());
		}

		public void ReplacemaximumversionAt(minimumversionType newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "maximumversion", index, newValue.ToString());
		}
		#endregion // maximumversion accessor methods

		#region maximumversion collection
        public maximumversionCollection	Mymaximumversions = new maximumversionCollection( );

        public class maximumversionCollection: IEnumerable
        {
            requirementType parent;
            public requirementType Parent
			{
				set
				{
					parent = value;
				}
			}
			public maximumversionEnumerator GetEnumerator() 
			{
				return new maximumversionEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class maximumversionEnumerator: IEnumerator 
        {
			int nIndex;
			requirementType parent;
			public maximumversionEnumerator(requirementType par) 
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
				return(nIndex < parent.maximumversionCount );
			}
			public minimumversionType  Current 
			{
				get 
				{
					return(parent.GetmaximumversionAt(nIndex));
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
	
        #endregion // maximumversion collection

        private void SetCollectionParents()
        {
            Mytype2s.Parent = this; 
            Mynames.Parent = this; 
            Myminimumversions.Parent = this; 
            Mymaximumversions.Parent = this; 
	}
}
}