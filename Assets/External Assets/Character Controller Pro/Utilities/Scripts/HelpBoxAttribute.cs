

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Lightbug.Utilities
{


public enum HelpBoxMessageType //Redefined to avoid using UnityEditor if possible
{
	None = 0,
	Info = 1,
	Warning = 2,
	Error = 3
}



[System.AttributeUsage( System.AttributeTargets.Field )]
public class HelpBoxAttribute : PropertyAttribute
{
	
	public string text;
    public HelpBoxMessageType messageType;

	public HelpBoxAttribute( string text , HelpBoxMessageType messageType )
	{		
		this.text = text;
        this.messageType = messageType;
	}   
	
	
}

#if UNITY_EDITOR

[CustomPropertyDrawer( typeof(HelpBoxAttribute))]
public class HelpBoxAttributeEditor : DecoratorDrawer
{	
	const float m_titleHeight = 30;

	GUIStyle m_style = new GUIStyle();
	Texture2D m_backgroundTexture = new Texture2D(1,1);

	Color m_backgroundColor;
	Color m_textColor;

	new HelpBoxAttribute attribute = null;

	public override float GetHeight()
	{
		attribute = base.attribute as HelpBoxAttribute;	
		return Mathf.Max( m_style.CalcSize( new GUIContent( attribute.text ) ).y , 40f );
	}
	

	public override void OnGUI( Rect position )
	{
        attribute = base.attribute as HelpBoxAttribute;				
		EditorGUI.HelpBox( position , attribute.text , (MessageType)attribute.messageType );		
	}

}

#endif

}


