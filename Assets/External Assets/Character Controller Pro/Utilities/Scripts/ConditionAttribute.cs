using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lightbug.Utilities
{

[System.AttributeUsage(System.AttributeTargets.Field)]
public class ConditionAttribute : PropertyAttribute
{
    public enum ConditionType
    {
        IsTrue ,
        IsFalse ,
        IsGreaterThan ,
        IsEqualTo ,
        IsLessThan ,
        HasReference
    }

    public enum VisibilityType
    {
        Hidden ,
        NotEditable
    }

    public string conditionAPropertyName = null;
    public string conditionBPropertyName = null;
    public ConditionType conditionType;
    public VisibilityType visibilityType;
    public float value;

    /// <summary>
    /// This attribute will determine the visibility of the target property based on some other property condition. Use this attribute if the target property 
    /// depends on some other property inside the class.
    /// </summary>
    /// <param name="conditionPropertyName">Name of the property used by the condition.</param>
    /// <param name="conditionType">The condition type.</param>
    /// <param name="visibilityType">The visibility action to perform if the condition is not met.</param>
    /// <param name="conditionValue">The condition argument value.</param>
    public ConditionAttribute( string conditionPropertyName , ConditionType conditionType , VisibilityType visibilityType = VisibilityType.Hidden , float conditionValue = 0f )
    {
        this.conditionAPropertyName = conditionPropertyName;
        this.conditionType = conditionType;
        this.visibilityType = visibilityType;
        this.value = conditionValue;

    }

    public ConditionAttribute( string conditionAPropertyName , string conditionBPropertyName , ConditionType conditionType , VisibilityType visibilityType = VisibilityType.Hidden , float conditionValue = 0f )
    {
        this.conditionAPropertyName = conditionAPropertyName;
        this.conditionBPropertyName = conditionBPropertyName;
        this.conditionType = conditionType;
        this.visibilityType = visibilityType;
        this.value = conditionValue;

    }
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ConditionAttribute))]
public class ConditionAttributeEditor : PropertyDrawer
{
	ConditionAttribute target;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{		
		if( target == null )
			target = attribute as ConditionAttribute;        

        bool result = CheckCondition( property );
        if( target.visibilityType == ConditionAttribute.VisibilityType.NotEditable )
        {
            GUI.enabled = result;
            EditorGUI.PropertyField( position , property , true );
            GUI.enabled = true;
        }
        else
        {
            if( result )
                EditorGUI.PropertyField( position , property , true );
        }
        

	}

    bool result = false;

    bool CheckCondition( SerializedProperty property )
    {
                
        bool evaluateTwoConditions = target.conditionBPropertyName != null;

        return evaluateTwoConditions ? 
            EvaluateCondition( property , target.conditionAPropertyName ) : 
            EvaluateCondition( property , target.conditionAPropertyName ) && EvaluateCondition( property , target.conditionBPropertyName );               
        
    }

    bool EvaluateCondition( SerializedProperty property , string conditionPropertyName )
    {
        SerializedProperty conditionProperty = property.serializedObject.FindProperty( target.conditionAPropertyName );    

        // if the "conditionProperty" is null, then the property is probably part of a plain C# serialized class. If so, then find the property root path
        // and look for the target condition property again.
        if( conditionProperty == null )
        {
            string propertyPath = property.propertyPath;
            int lastIndex = propertyPath.LastIndexOf('.');           

            if( lastIndex == -1 )
                return true;      

            
            string propertyParentPath = propertyPath.Substring( 0 , lastIndex );

            conditionProperty = property.serializedObject.FindProperty( propertyParentPath ).FindPropertyRelative( conditionPropertyName );

            if( conditionProperty == null )
                return true;

        }
            
        
        result = false;

        SerializedPropertyType conditionPropertyType = conditionProperty.propertyType;

        if( conditionPropertyType == SerializedPropertyType.Boolean )
        {
            if( target.conditionType == ConditionAttribute.ConditionType.IsTrue )
                result = conditionProperty.boolValue;
            else if( target.conditionType == ConditionAttribute.ConditionType.IsFalse )
                result = !conditionProperty.boolValue;
            
        }
        else if( conditionPropertyType == SerializedPropertyType.Float )
        {
                
                float conditionPropertyFloatValue = conditionProperty.floatValue;
                float argumentFloatValue = target.value;

                switch( target.conditionType )
                {
                    case ConditionAttribute.ConditionType.IsTrue:
                        result = conditionPropertyFloatValue != 0f;
                        break;
                    case ConditionAttribute.ConditionType.IsFalse:
                        result = conditionPropertyFloatValue == 0f;
                        break;
                    case ConditionAttribute.ConditionType.IsGreaterThan:
                        result = conditionPropertyFloatValue > argumentFloatValue;
                        break;
                    case ConditionAttribute.ConditionType.IsEqualTo:
                        result = conditionPropertyFloatValue == argumentFloatValue;
                        break;
                    case ConditionAttribute.ConditionType.IsLessThan:
                        result = conditionPropertyFloatValue < argumentFloatValue;
                        break;
                }
                
        }
        else if( conditionPropertyType == SerializedPropertyType.Integer || conditionPropertyType == SerializedPropertyType.Enum )
        {
            int conditionPropertyIntValue = conditionProperty.intValue;
            int argumentIntValue = (int)target.value;

            switch( target.conditionType )
            {
                case ConditionAttribute.ConditionType.IsTrue:
                    result = conditionPropertyIntValue != 0;
                    break;
                case ConditionAttribute.ConditionType.IsFalse:
                    result = conditionPropertyIntValue == 0;
                    break;
                case ConditionAttribute.ConditionType.IsGreaterThan:
                    result = conditionPropertyIntValue > argumentIntValue;
                    break;
                case ConditionAttribute.ConditionType.IsEqualTo:
                    result = conditionPropertyIntValue == argumentIntValue;
                    break;
                case ConditionAttribute.ConditionType.IsLessThan:
                    result = conditionPropertyIntValue < argumentIntValue;
                    break;
            }
            
        }    
        else if( conditionPropertyType == SerializedPropertyType.ObjectReference )
        {
            UnityEngine.Object conditionPropertyObjectValue = conditionProperty.objectReferenceValue;
            result = conditionPropertyObjectValue != null;
            
        }    
        
        return result;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if( target == null )
			target = attribute as ConditionAttribute;    
        
        return !result && target.visibilityType == ConditionAttribute.VisibilityType.Hidden ? 0f : EditorGUI.GetPropertyHeight( property );
    }

    
		
}

#endif

}

