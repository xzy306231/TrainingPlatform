//
// primaryObjectiveType.cs.cs
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

namespace adlcp_v1p3.imsss_v1p0
{
	public class primaryObjectiveType : Altova.Node
	{
		#region Forward constructors
		public primaryObjectiveType() : base() { SetCollectionParents(); }
		public primaryObjectiveType(XmlDocument doc) : base(doc) { SetCollectionParents(); }
		public primaryObjectiveType(XmlNode node) : base(node) { SetCollectionParents(); }
		public primaryObjectiveType(Altova.Node node) : base(node) { SetCollectionParents(); }
		#endregion // Forward constructors

		public override void AdjustPrefix()
		{
			int nCount;

			nCount = DomChildCount(NodeType.Attribute, "", "satisfiedByMeasure");
			for (int i = 0; i < nCount; i++)
			{
				XmlNode DOMNode = GetDomChildAt(NodeType.Attribute, "", "satisfiedByMeasure", i);
				InternalAdjustPrefix(DOMNode, false);
			}

			nCount = DomChildCount(NodeType.Attribute, "", "objectiveID");
			for (int i = 0; i < nCount; i++)
			{
				XmlNode DOMNode = GetDomChildAt(NodeType.Attribute, "", "objectiveID", i);
				InternalAdjustPrefix(DOMNode, false);
			}

			nCount = DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsss", "minNormalizedMeasure");
			for (int i = 0; i < nCount; i++)
			{
				XmlNode DOMNode = GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsss", "minNormalizedMeasure", i);
				InternalAdjustPrefix(DOMNode, true);
			}

			nCount = DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsss", "mapInfo");
			for (int i = 0; i < nCount; i++)
			{
				XmlNode DOMNode = GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsss", "mapInfo", i);
				InternalAdjustPrefix(DOMNode, true);
				new mapInfoType(DOMNode).AdjustPrefix();
			}
		}


		#region satisfiedByMeasure accessor methods
		public int GetsatisfiedByMeasureMinCount()
		{
			return 0;
		}

		public int satisfiedByMeasureMinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetsatisfiedByMeasureMaxCount()
		{
			return 1;
		}

		public int satisfiedByMeasureMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetsatisfiedByMeasureCount()
		{
			return DomChildCount(NodeType.Attribute, "", "satisfiedByMeasure");
		}

		public int satisfiedByMeasureCount
		{
			get
			{
				return DomChildCount(NodeType.Attribute, "", "satisfiedByMeasure");
			}
		}

		public bool HassatisfiedByMeasure()
		{
			return HasDomChild(NodeType.Attribute, "", "satisfiedByMeasure");
		}

		public SchemaBoolean GetsatisfiedByMeasureAt(int index)
		{
			return new SchemaBoolean(GetDomNodeValue(GetDomChildAt(NodeType.Attribute, "", "satisfiedByMeasure", index)));
		}

		public SchemaBoolean GetsatisfiedByMeasure()
		{
			return GetsatisfiedByMeasureAt(0);
		}

		public SchemaBoolean satisfiedByMeasure
		{
			get
			{
				return GetsatisfiedByMeasureAt(0);
			}
		}

		public void RemovesatisfiedByMeasureAt(int index)
		{
			RemoveDomChildAt(NodeType.Attribute, "", "satisfiedByMeasure", index);
		}

		public void RemovesatisfiedByMeasure()
		{
			while (HassatisfiedByMeasure())
				RemovesatisfiedByMeasureAt(0);
		}

		public void AddsatisfiedByMeasure(SchemaBoolean newValue)
		{
			AppendDomChild(NodeType.Attribute, "", "satisfiedByMeasure", newValue.ToString());
		}

		public void InsertsatisfiedByMeasureAt(SchemaBoolean newValue, int index)
		{
			InsertDomChildAt(NodeType.Attribute, "", "satisfiedByMeasure", index, newValue.ToString());
		}

		public void ReplacesatisfiedByMeasureAt(SchemaBoolean newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Attribute, "", "satisfiedByMeasure", index, newValue.ToString());
		}
		#endregion // satisfiedByMeasure accessor methods

		#region satisfiedByMeasure collection
        public satisfiedByMeasureCollection	MysatisfiedByMeasures = new satisfiedByMeasureCollection( );

        public class satisfiedByMeasureCollection: IEnumerable
        {
            primaryObjectiveType parent;
            public primaryObjectiveType Parent
			{
				set
				{
					parent = value;
				}
			}
			public satisfiedByMeasureEnumerator GetEnumerator() 
			{
				return new satisfiedByMeasureEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class satisfiedByMeasureEnumerator: IEnumerator 
        {
			int nIndex;
			primaryObjectiveType parent;
			public satisfiedByMeasureEnumerator(primaryObjectiveType par) 
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
				return(nIndex < parent.satisfiedByMeasureCount );
			}
			public SchemaBoolean  Current 
			{
				get 
				{
					return(parent.GetsatisfiedByMeasureAt(nIndex));
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
	
        #endregion // satisfiedByMeasure collection

		#region objectiveID accessor methods
		public int GetobjectiveIDMinCount()
		{
			return 0;
		}

		public int objectiveIDMinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetobjectiveIDMaxCount()
		{
			return 1;
		}

		public int objectiveIDMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetobjectiveIDCount()
		{
			return DomChildCount(NodeType.Attribute, "", "objectiveID");
		}

		public int objectiveIDCount
		{
			get
			{
				return DomChildCount(NodeType.Attribute, "", "objectiveID");
			}
		}

		public bool HasobjectiveID()
		{
			return HasDomChild(NodeType.Attribute, "", "objectiveID");
		}

		public SchemaString GetobjectiveIDAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Attribute, "", "objectiveID", index)));
		}

		public SchemaString GetobjectiveID()
		{
			return GetobjectiveIDAt(0);
		}

		public SchemaString objectiveID
		{
			get
			{
				return GetobjectiveIDAt(0);
			}
		}

		public void RemoveobjectiveIDAt(int index)
		{
			RemoveDomChildAt(NodeType.Attribute, "", "objectiveID", index);
		}

		public void RemoveobjectiveID()
		{
			while (HasobjectiveID())
				RemoveobjectiveIDAt(0);
		}

		public void AddobjectiveID(SchemaString newValue)
		{
			AppendDomChild(NodeType.Attribute, "", "objectiveID", newValue.ToString());
		}

		public void InsertobjectiveIDAt(SchemaString newValue, int index)
		{
			InsertDomChildAt(NodeType.Attribute, "", "objectiveID", index, newValue.ToString());
		}

		public void ReplaceobjectiveIDAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Attribute, "", "objectiveID", index, newValue.ToString());
		}
		#endregion // objectiveID accessor methods

		#region objectiveID collection
        public objectiveIDCollection	MyobjectiveIDs = new objectiveIDCollection( );

        public class objectiveIDCollection: IEnumerable
        {
            primaryObjectiveType parent;
            public primaryObjectiveType Parent
			{
				set
				{
					parent = value;
				}
			}
			public objectiveIDEnumerator GetEnumerator() 
			{
				return new objectiveIDEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class objectiveIDEnumerator: IEnumerator 
        {
			int nIndex;
			primaryObjectiveType parent;
			public objectiveIDEnumerator(primaryObjectiveType par) 
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
				return(nIndex < parent.objectiveIDCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetobjectiveIDAt(nIndex));
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
	
        #endregion // objectiveID collection

		#region minNormalizedMeasure accessor methods
		public int GetminNormalizedMeasureMinCount()
		{
			return 0;
		}

		public int minNormalizedMeasureMinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetminNormalizedMeasureMaxCount()
		{
			return 1;
		}

		public int minNormalizedMeasureMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetminNormalizedMeasureCount()
		{
			return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsss", "minNormalizedMeasure");
		}

		public int minNormalizedMeasureCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsss", "minNormalizedMeasure");
			}
		}

		public bool HasminNormalizedMeasure()
		{
			return HasDomChild(NodeType.Element, "http://www.imsglobal.org/xsd/imsss", "minNormalizedMeasure");
		}

		public measureType GetminNormalizedMeasureAt(int index)
		{
			return new measureType(GetDomNodeValue(GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsss", "minNormalizedMeasure", index)));
		}

		public measureType GetminNormalizedMeasure()
		{
			return GetminNormalizedMeasureAt(0);
		}

		public measureType minNormalizedMeasure
		{
			get
			{
				return GetminNormalizedMeasureAt(0);
			}
		}

		public void RemoveminNormalizedMeasureAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsss", "minNormalizedMeasure", index);
		}

		public void RemoveminNormalizedMeasure()
		{
			while (HasminNormalizedMeasure())
				RemoveminNormalizedMeasureAt(0);
		}

		public void AddminNormalizedMeasure(measureType newValue)
		{
			AppendDomChild(NodeType.Element, "http://www.imsglobal.org/xsd/imsss", "minNormalizedMeasure", newValue.ToString());
		}

		public void InsertminNormalizedMeasureAt(measureType newValue, int index)
		{
			InsertDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsss", "minNormalizedMeasure", index, newValue.ToString());
		}

		public void ReplaceminNormalizedMeasureAt(measureType newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsss", "minNormalizedMeasure", index, newValue.ToString());
		}
		#endregion // minNormalizedMeasure accessor methods

		#region minNormalizedMeasure collection
        public minNormalizedMeasureCollection	MyminNormalizedMeasures = new minNormalizedMeasureCollection( );

        public class minNormalizedMeasureCollection: IEnumerable
        {
            primaryObjectiveType parent;
            public primaryObjectiveType Parent
			{
				set
				{
					parent = value;
				}
			}
			public minNormalizedMeasureEnumerator GetEnumerator() 
			{
				return new minNormalizedMeasureEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class minNormalizedMeasureEnumerator: IEnumerator 
        {
			int nIndex;
			primaryObjectiveType parent;
			public minNormalizedMeasureEnumerator(primaryObjectiveType par) 
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
				return(nIndex < parent.minNormalizedMeasureCount );
			}
			public measureType  Current 
			{
				get 
				{
					return(parent.GetminNormalizedMeasureAt(nIndex));
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
	
        #endregion // minNormalizedMeasure collection

		#region mapInfo accessor methods
		public int GetmapInfoMinCount()
		{
			return 0;
		}

		public int mapInfoMinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetmapInfoMaxCount()
		{
			return Int32.MaxValue;
		}

		public int mapInfoMaxCount
		{
			get
			{
				return Int32.MaxValue;
			}
		}

		public int GetmapInfoCount()
		{
			return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsss", "mapInfo");
		}

		public int mapInfoCount
		{
			get
			{
				return DomChildCount(NodeType.Element, "http://www.imsglobal.org/xsd/imsss", "mapInfo");
			}
		}

		public bool HasmapInfo()
		{
			return HasDomChild(NodeType.Element, "http://www.imsglobal.org/xsd/imsss", "mapInfo");
		}

		public mapInfoType GetmapInfoAt(int index)
		{
			return new mapInfoType(GetDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsss", "mapInfo", index));
		}

		public mapInfoType GetmapInfo()
		{
			return GetmapInfoAt(0);
		}

		public mapInfoType mapInfo
		{
			get
			{
				return GetmapInfoAt(0);
			}
		}

		public void RemovemapInfoAt(int index)
		{
			RemoveDomChildAt(NodeType.Element, "http://www.imsglobal.org/xsd/imsss", "mapInfo", index);
		}

		public void RemovemapInfo()
		{
			while (HasmapInfo())
				RemovemapInfoAt(0);
		}

		public void AddmapInfo(mapInfoType newValue)
		{
			AppendDomElement("http://www.imsglobal.org/xsd/imsss", "mapInfo", newValue);
		}

		public void InsertmapInfoAt(mapInfoType newValue, int index)
		{
			InsertDomElementAt("http://www.imsglobal.org/xsd/imsss", "mapInfo", index, newValue);
		}

		public void ReplacemapInfoAt(mapInfoType newValue, int index)
		{
			ReplaceDomElementAt("http://www.imsglobal.org/xsd/imsss", "mapInfo", index, newValue);
		}
		#endregion // mapInfo accessor methods

		#region mapInfo collection
        public mapInfoCollection	MymapInfos = new mapInfoCollection( );

        public class mapInfoCollection: IEnumerable
        {
            primaryObjectiveType parent;
            public primaryObjectiveType Parent
			{
				set
				{
					parent = value;
				}
			}
			public mapInfoEnumerator GetEnumerator() 
			{
				return new mapInfoEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class mapInfoEnumerator: IEnumerator 
        {
			int nIndex;
			primaryObjectiveType parent;
			public mapInfoEnumerator(primaryObjectiveType par) 
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
				return(nIndex < parent.mapInfoCount );
			}
			public mapInfoType  Current 
			{
				get 
				{
					return(parent.GetmapInfoAt(nIndex));
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
	
        #endregion // mapInfo collection

        private void SetCollectionParents()
        {
            MysatisfiedByMeasures.Parent = this; 
            MyobjectiveIDs.Parent = this; 
            MyminNormalizedMeasures.Parent = this; 
            MymapInfos.Parent = this; 
	}
}
}
