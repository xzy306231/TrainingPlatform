//
// ruleConditionType.cs.cs
//
// This file was generated by XMLSPY 2004 Enterprise Edition.
//
// YOU SHOULD NOT MODIFY THIS FILE, BECAUSE IT WILL BE
// OVERWRITTEN WHEN YOU RE-RUN CODE GENERATION.
//
// Refer to the XMLSPY Documentation for further details.
// http://www.altova.com/xmlspy
//


using System.Collections;
using System.Xml;
using adlcp_v1p3.Altova.Types;

namespace adlcp_v1p3.imsss_v1p0
{
	public class ruleConditionType : Altova.Node
	{
		#region Forward constructors
		public ruleConditionType() : base() { SetCollectionParents(); }
		public ruleConditionType(XmlDocument doc) : base(doc) { SetCollectionParents(); }
		public ruleConditionType(XmlNode node) : base(node) { SetCollectionParents(); }
		public ruleConditionType(Altova.Node node) : base(node) { SetCollectionParents(); }
		#endregion // Forward constructors

		public override void AdjustPrefix()
		{
			int nCount;

			nCount = DomChildCount(NodeType.Attribute, "", "referencedObjective");
			for (int i = 0; i < nCount; i++)
			{
				XmlNode DOMNode = GetDomChildAt(NodeType.Attribute, "", "referencedObjective", i);
				InternalAdjustPrefix(DOMNode, false);
			}

			nCount = DomChildCount(NodeType.Attribute, "", "measureThreshold");
			for (int i = 0; i < nCount; i++)
			{
				XmlNode DOMNode = GetDomChildAt(NodeType.Attribute, "", "measureThreshold", i);
				InternalAdjustPrefix(DOMNode, false);
			}

			nCount = DomChildCount(NodeType.Attribute, "", "operator");
			for (int i = 0; i < nCount; i++)
			{
				XmlNode DOMNode = GetDomChildAt(NodeType.Attribute, "", "operator", i);
				InternalAdjustPrefix(DOMNode, false);
			}

			nCount = DomChildCount(NodeType.Attribute, "", "condition");
			for (int i = 0; i < nCount; i++)
			{
				XmlNode DOMNode = GetDomChildAt(NodeType.Attribute, "", "condition", i);
				InternalAdjustPrefix(DOMNode, false);
			}
		}


		#region referencedObjective accessor methods
		public int GetreferencedObjectiveMinCount()
		{
			return 0;
		}

		public int referencedObjectiveMinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetreferencedObjectiveMaxCount()
		{
			return 1;
		}

		public int referencedObjectiveMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetreferencedObjectiveCount()
		{
			return DomChildCount(NodeType.Attribute, "", "referencedObjective");
		}

		public int referencedObjectiveCount
		{
			get
			{
				return DomChildCount(NodeType.Attribute, "", "referencedObjective");
			}
		}

		public bool HasreferencedObjective()
		{
			return HasDomChild(NodeType.Attribute, "", "referencedObjective");
		}

		public SchemaString GetreferencedObjectiveAt(int index)
		{
			return new SchemaString(GetDomNodeValue(GetDomChildAt(NodeType.Attribute, "", "referencedObjective", index)));
		}

		public SchemaString GetreferencedObjective()
		{
			return GetreferencedObjectiveAt(0);
		}

		public SchemaString referencedObjective
		{
			get
			{
				return GetreferencedObjectiveAt(0);
			}
		}

		public void RemovereferencedObjectiveAt(int index)
		{
			RemoveDomChildAt(NodeType.Attribute, "", "referencedObjective", index);
		}

		public void RemovereferencedObjective()
		{
			while (HasreferencedObjective())
				RemovereferencedObjectiveAt(0);
		}

		public void AddreferencedObjective(SchemaString newValue)
		{
			AppendDomChild(NodeType.Attribute, "", "referencedObjective", newValue.ToString());
		}

		public void InsertreferencedObjectiveAt(SchemaString newValue, int index)
		{
			InsertDomChildAt(NodeType.Attribute, "", "referencedObjective", index, newValue.ToString());
		}

		public void ReplacereferencedObjectiveAt(SchemaString newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Attribute, "", "referencedObjective", index, newValue.ToString());
		}
		#endregion // referencedObjective accessor methods

		#region referencedObjective collection
        public referencedObjectiveCollection	MyreferencedObjectives = new referencedObjectiveCollection( );

        public class referencedObjectiveCollection: IEnumerable
        {
            ruleConditionType parent;
            public ruleConditionType Parent
			{
				set
				{
					parent = value;
				}
			}
			public referencedObjectiveEnumerator GetEnumerator() 
			{
				return new referencedObjectiveEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class referencedObjectiveEnumerator: IEnumerator 
        {
			int nIndex;
			ruleConditionType parent;
			public referencedObjectiveEnumerator(ruleConditionType par) 
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
				return(nIndex < parent.referencedObjectiveCount );
			}
			public SchemaString  Current 
			{
				get 
				{
					return(parent.GetreferencedObjectiveAt(nIndex));
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
	
        #endregion // referencedObjective collection

		#region measureThreshold accessor methods
		public int GetmeasureThresholdMinCount()
		{
			return 0;
		}

		public int measureThresholdMinCount
		{
			get
			{
				return 0;
			}
		}

		public int GetmeasureThresholdMaxCount()
		{
			return 1;
		}

		public int measureThresholdMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetmeasureThresholdCount()
		{
			return DomChildCount(NodeType.Attribute, "", "measureThreshold");
		}

		public int measureThresholdCount
		{
			get
			{
				return DomChildCount(NodeType.Attribute, "", "measureThreshold");
			}
		}

		public bool HasmeasureThreshold()
		{
			return HasDomChild(NodeType.Attribute, "", "measureThreshold");
		}

		public measureType GetmeasureThresholdAt(int index)
		{
			return new measureType(GetDomNodeValue(GetDomChildAt(NodeType.Attribute, "", "measureThreshold", index)));
		}

		public measureType GetmeasureThreshold()
		{
			return GetmeasureThresholdAt(0);
		}

		public measureType measureThreshold
		{
			get
			{
				return GetmeasureThresholdAt(0);
			}
		}

		public void RemovemeasureThresholdAt(int index)
		{
			RemoveDomChildAt(NodeType.Attribute, "", "measureThreshold", index);
		}

		public void RemovemeasureThreshold()
		{
			while (HasmeasureThreshold())
				RemovemeasureThresholdAt(0);
		}

		public void AddmeasureThreshold(measureType newValue)
		{
			AppendDomChild(NodeType.Attribute, "", "measureThreshold", newValue.ToString());
		}

		public void InsertmeasureThresholdAt(measureType newValue, int index)
		{
			InsertDomChildAt(NodeType.Attribute, "", "measureThreshold", index, newValue.ToString());
		}

		public void ReplacemeasureThresholdAt(measureType newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Attribute, "", "measureThreshold", index, newValue.ToString());
		}
		#endregion // measureThreshold accessor methods

		#region measureThreshold collection
        public measureThresholdCollection	MymeasureThresholds = new measureThresholdCollection( );

        public class measureThresholdCollection: IEnumerable
        {
            ruleConditionType parent;
            public ruleConditionType Parent
			{
				set
				{
					parent = value;
				}
			}
			public measureThresholdEnumerator GetEnumerator() 
			{
				return new measureThresholdEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class measureThresholdEnumerator: IEnumerator 
        {
			int nIndex;
			ruleConditionType parent;
			public measureThresholdEnumerator(ruleConditionType par) 
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
				return(nIndex < parent.measureThresholdCount );
			}
			public measureType  Current 
			{
				get 
				{
					return(parent.GetmeasureThresholdAt(nIndex));
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
	
        #endregion // measureThreshold collection

		#region operator2 accessor methods
		public int Getoperator2MinCount()
		{
			return 0;
		}

		public int operator2MinCount
		{
			get
			{
				return 0;
			}
		}

		public int Getoperator2MaxCount()
		{
			return 1;
		}

		public int operator2MaxCount
		{
			get
			{
				return 1;
			}
		}

		public int Getoperator2Count()
		{
			return DomChildCount(NodeType.Attribute, "", "operator");
		}

		public int operator2Count
		{
			get
			{
				return DomChildCount(NodeType.Attribute, "", "operator");
			}
		}

		public bool Hasoperator2()
		{
			return HasDomChild(NodeType.Attribute, "", "operator");
		}

		public conditionOperatorType Getoperator2At(int index)
		{
			return new conditionOperatorType(GetDomNodeValue(GetDomChildAt(NodeType.Attribute, "", "operator", index)));
		}

		public conditionOperatorType Getoperator2()
		{
			return Getoperator2At(0);
		}

		public conditionOperatorType operator2
		{
			get
			{
				return Getoperator2At(0);
			}
		}

		public void Removeoperator2At(int index)
		{
			RemoveDomChildAt(NodeType.Attribute, "", "operator", index);
		}

		public void Removeoperator2()
		{
			while (Hasoperator2())
				Removeoperator2At(0);
		}

		public void Addoperator2(conditionOperatorType newValue)
		{
			AppendDomChild(NodeType.Attribute, "", "operator", newValue.ToString());
		}

		public void Insertoperator2At(conditionOperatorType newValue, int index)
		{
			InsertDomChildAt(NodeType.Attribute, "", "operator", index, newValue.ToString());
		}

		public void Replaceoperator2At(conditionOperatorType newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Attribute, "", "operator", index, newValue.ToString());
		}
		#endregion // operator2 accessor methods

		#region operator2 collection
        public operator2Collection	Myoperator2s = new operator2Collection( );

        public class operator2Collection: IEnumerable
        {
            ruleConditionType parent;
            public ruleConditionType Parent
			{
				set
				{
					parent = value;
				}
			}
			public operator2Enumerator GetEnumerator() 
			{
				return new operator2Enumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class operator2Enumerator: IEnumerator 
        {
			int nIndex;
			ruleConditionType parent;
			public operator2Enumerator(ruleConditionType par) 
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
				return(nIndex < parent.operator2Count );
			}
			public conditionOperatorType  Current 
			{
				get 
				{
					return(parent.Getoperator2At(nIndex));
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
	
        #endregion // operator2 collection

		#region condition accessor methods
		public int GetconditionMinCount()
		{
			return 1;
		}

		public int conditionMinCount
		{
			get
			{
				return 1;
			}
		}

		public int GetconditionMaxCount()
		{
			return 1;
		}

		public int conditionMaxCount
		{
			get
			{
				return 1;
			}
		}

		public int GetconditionCount()
		{
			return DomChildCount(NodeType.Attribute, "", "condition");
		}

		public int conditionCount
		{
			get
			{
				return DomChildCount(NodeType.Attribute, "", "condition");
			}
		}

		public bool Hascondition()
		{
			return HasDomChild(NodeType.Attribute, "", "condition");
		}

		public sequencingRuleConditionType GetconditionAt(int index)
		{
			return new sequencingRuleConditionType(GetDomNodeValue(GetDomChildAt(NodeType.Attribute, "", "condition", index)));
		}

		public sequencingRuleConditionType Getcondition()
		{
			return GetconditionAt(0);
		}

		public sequencingRuleConditionType condition
		{
			get
			{
				return GetconditionAt(0);
			}
		}

		public void RemoveconditionAt(int index)
		{
			RemoveDomChildAt(NodeType.Attribute, "", "condition", index);
		}

		public void Removecondition()
		{
			while (Hascondition())
				RemoveconditionAt(0);
		}

		public void Addcondition(sequencingRuleConditionType newValue)
		{
			AppendDomChild(NodeType.Attribute, "", "condition", newValue.ToString());
		}

		public void InsertconditionAt(sequencingRuleConditionType newValue, int index)
		{
			InsertDomChildAt(NodeType.Attribute, "", "condition", index, newValue.ToString());
		}

		public void ReplaceconditionAt(sequencingRuleConditionType newValue, int index)
		{
			ReplaceDomChildAt(NodeType.Attribute, "", "condition", index, newValue.ToString());
		}
		#endregion // condition accessor methods

		#region condition collection
        public conditionCollection	Myconditions = new conditionCollection( );

        public class conditionCollection: IEnumerable
        {
            ruleConditionType parent;
            public ruleConditionType Parent
			{
				set
				{
					parent = value;
				}
			}
			public conditionEnumerator GetEnumerator() 
			{
				return new conditionEnumerator(parent);
			}
		
			IEnumerator IEnumerable.GetEnumerator() 
			{
				return GetEnumerator();
			}
        }

        public class conditionEnumerator: IEnumerator 
        {
			int nIndex;
			ruleConditionType parent;
			public conditionEnumerator(ruleConditionType par) 
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
				return(nIndex < parent.conditionCount );
			}
			public sequencingRuleConditionType  Current 
			{
				get 
				{
					return(parent.GetconditionAt(nIndex));
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
	
        #endregion // condition collection

        private void SetCollectionParents()
        {
            MyreferencedObjectives.Parent = this; 
            MymeasureThresholds.Parent = this; 
            Myoperator2s.Parent = this; 
            Myconditions.Parent = this; 
	}
}
}
