//
// durationType.cs.cs
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
	public class durationType : Altova.Node
	{
		#region Forward constructors
		public durationType() : base() { SetCollectionParents(); }
		public durationType(XmlDocument doc) : base(doc) { SetCollectionParents(); }
		public durationType(XmlNode node) : base(node) { SetCollectionParents(); }
		public durationType(Altova.Node node) : base(node) { SetCollectionParents(); }
		#endregion // Forward constructors

		public override void AdjustPrefix()
		{
			int nCount;

			nCount = DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "datetime");
			for (int i = 0; i < nCount; i++)
			{
				XmlNode DOMNode = GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "datetime", i);
				InternalAdjustPrefix(DOMNode, true);
			}

			nCount = DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "description");
			for (int i = 0; i < nCount; i++)
			{
				XmlNode DOMNode = GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "description", i);
				InternalAdjustPrefix(DOMNode, true);
				new descriptionType(DOMNode).AdjustPrefix();
			}
		}


		#region datetime accessor methods
		public int GetdatetimeMinCount()
		{
			return 0;
		}

		public int datetimeMinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetdatetimeMaxCount()
		{
			return 1;
		}

		public int datetimeMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetdatetimeCount()
		{
			return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "datetime");
		}

		public int datetimeCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "datetime");
			}
		}

		public bool Hasdatetime()
		{
			return HasDomChild(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "datetime");
		}

		public datetimeType GetdatetimeAt(int index)
		{
			return new datetimeType(GetDomNodeValue(GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "datetime", index)));
		}

		public datetimeType Getdatetime()
		{
			return GetdatetimeAt(0);
		}

		public datetimeType datetime
		{
			get
			{
				return GetdatetimeAt(0);
			}
		}

		public void RemovedatetimeAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "datetime", index);
		}

		public void Removedatetime()
		{
			while (Hasdatetime())
				RemovedatetimeAt(0);
		}

		public void Adddatetime(datetimeType newValue)
		{
			AppendDomChild(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "datetime", newValue.ToString());
		}

		public void InsertdatetimeAt(datetimeType newValue, int index)
		{
			InsertDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "datetime", index, newValue.ToString());
		}

		public void ReplacedatetimeAt(datetimeType newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsmd_rootv1p2p1", "datetime", index, newValue.ToString());
		}
		#endregion // datetime accessor methods

		#region datetime collection
        public datetimeCollection	Mydatetimes = new datetimeCollection( );

        public class datetimeCollection: IEnumerable
        {
            durationType parent;
            public durationType Parent
			{
				set
				{
					parent = value;
				}
			}
			public datetimeEnumerator GetEnumerator() 
			{
				return new datetimeEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class datetimeEnumerator: IEnumerator 
        {
			int nIndex;
			durationType parent;
			public datetimeEnumerator(durationType par) 
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
				return(nIndex < parent.datetimeCount );
			}
			public datetimeType  Current 
			{
				get 
				{
					return(parent.GetdatetimeAt(nIndex));
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
	
        #endregion // datetime collection

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
            durationType parent;
            public durationType Parent
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
			durationType parent;
			public descriptionEnumerator(durationType par) 
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

        private void SetCollectionParents()
        {
            Mydatetimes.Parent = this; 
            Mydescriptions.Parent = this; 
	}
}
}
