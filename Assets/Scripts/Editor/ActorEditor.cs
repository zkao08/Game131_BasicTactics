using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Actor))]
public class ActorEditor : Editor
{
	bool isImmunitiesShowing = true;
	bool isBoardPosShowing = true;
	bool enemy = true;

	public override void OnInspectorGUI()
	{
		Actor myActor = target as Actor;

		//Actor name can not be empty, default to name of game object
		myActor.actorName = EditorGUILayout.TextField("Actor Name", myActor.actorName);
		if (myActor.actorName == "")
		{
			myActor.actorName = myActor.name;
		}

		EditorGUILayout.Space();

		//Max HP must be > 0
		int newHP = EditorGUILayout.DelayedIntField("Max HP", myActor.maxHitPoints);
		if (newHP > 0)
		{
			myActor.maxHitPoints = newHP;
		}

		//Initiative must be between 10 and 100 in steps of 5
		const int STEP = 5;
		myActor.initiative = STEP * (EditorGUILayout.IntSlider("Initiative", myActor.initiative, 10, 100) / STEP);

		//Immunities
		Actor.ActionSource[] sourceValues = Enum.GetValues(typeof(Actor.ActionSource)) as Actor.ActionSource[];
		string[] sourceNames = Enum.GetNames(typeof(Actor.ActionSource));
		SelectionList<Actor.ActionSource> sources = new SelectionList<Actor.ActionSource>(sourceValues, sourceNames);

		isImmunitiesShowing = EditorGUILayout.Foldout(isImmunitiesShowing, "Immunities");
		if (isImmunitiesShowing)
		{
			myActor.immunities = sources.CheckboxList("Immunities", myActor.immunities, 3);
		}

		EditorGUILayout.Space();

		//Action Effect
		Actor.ActionEffect[] effectValues = Enum.GetValues(typeof(Actor.ActionEffect)) as Actor.ActionEffect[];
		string[] effectNames = Enum.GetNames(typeof(Actor.ActionEffect));
		SelectionList<Actor.ActionEffect> effects = new SelectionList<Actor.ActionEffect>(effectValues, effectNames);
		
		myActor.actionEffect = effects.RadioList("Action Effect", myActor.actionEffect, 3);

		//Damage & Chance to hit
		if(myActor.actionEffect == Actor.ActionEffect.Normal)
		{
			myActor.damage = EditorGUILayout.IntSlider("Damage", myActor.damage, 0, 180);
			myActor.percentChanceToHit = EditorGUILayout.IntSlider("Percent Chance to Hit", myActor.percentChanceToHit, 0, 100);
		}
		else if(myActor.actionEffect == Actor.ActionEffect.Heal)
		{
			myActor.damage = EditorGUILayout.IntSlider("Damage", myActor.damage, 0, 180);
			myActor.percentChanceToHit = 100;
		}
		else
		{
			myActor.damage = 0;
			myActor.percentChanceToHit = EditorGUILayout.IntSlider("Percent Chance to Hit", myActor.percentChanceToHit, 0, 100);
		}

		//Action Effect Source
		myActor.actionEffectSource = (Actor.ActionSource)EditorGUILayout.EnumPopup("Action Effect Source", myActor.actionEffectSource);

		//Action Target
		myActor.actionTarget = (Actor.ActionTarget)EditorGUILayout.EnumPopup("Action Target", myActor.actionTarget);

		//Target Select Rule
		myActor.targetSelectionRule = (Actor.TargetSelectionRule)EditorGUILayout.EnumPopup("Target Selection Rule", myActor.targetSelectionRule);

		//Current Target
		EditorGUILayout.LabelField("Current Target:", myActor.currentTarget.name);

		EditorGUILayout.Space();

		//Board Position
		Actor.Position[] positionValues = Enum.GetValues(typeof(Actor.Position)) as Actor.Position[];
		string[] positionNames = Enum.GetNames(typeof(Actor.Position));
		SelectionList<Actor.Position> positions = new SelectionList<Actor.Position>(positionValues, positionNames);

		isBoardPosShowing = EditorGUILayout.Foldout(isBoardPosShowing, "Board Position");
		if (isBoardPosShowing)
		{
			myActor.boardPosition = positions.PositionGrid("Board Position", myActor.boardPosition, 3);
		}
	}
}

/** @author Steve Smith */
class SelectionList<T> where T : IComparable
{
	int f = 9;
	T[] _values;
	string[] _labels;
	T _selectedValue;

	public T[] CheckboxList(string label, T[] initialSelections, int itemsPerCol)
	{
		List<T> selectedValues = new List<T>();
		List<int> initialSelectedIndexes = new List<int>();
		for (int i = 0; i < _values.Length; i++)
		{
			for (int j = 0; j < initialSelections.Length; j++)
			{
				if (_values[i].CompareTo(initialSelections[j]) == 0) initialSelectedIndexes.Add(i);
			}
		}

		EditorGUILayout.BeginVertical();

		EditorGUILayout.BeginHorizontal();
		for (int r = 0; r < _values.Length; r += itemsPerCol)
		{
			EditorGUILayout.BeginVertical();
			for (int i = r; i < r + itemsPerCol && i < _values.Length; i++)
			{
				if (GUILayout.Toggle(initialSelectedIndexes.Contains(i), _labels[i]))
				{
					selectedValues.Add(_values[i]);
				}
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndVertical();

		return selectedValues.ToArray();

	}

	public T RadioList(string label, T initialSelection, int itemsPerRow)
	{
		T originalSelectedValue = _selectedValue;
		_selectedValue = initialSelection;
		bool anyChecked = false;

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(label, GUILayout.MaxWidth(100));

		EditorGUILayout.BeginVertical();
		for (int r = 0; r < _values.Length; r += itemsPerRow)
		{
			EditorGUILayout.BeginHorizontal();
			for (int i = r; i < r + itemsPerRow && i < _values.Length; i++)
			{
				if (_values[i].CompareTo(initialSelection) == 0) originalSelectedValue = initialSelection;
				if (GUILayout.Toggle(_values[i].CompareTo(_selectedValue) == 0, _labels[i]))
				{
					_selectedValue = _values[i];
					anyChecked = true;
				}
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();

		EditorGUILayout.EndHorizontal();

		if (!anyChecked) _selectedValue = originalSelectedValue;
		return _selectedValue;
	}

	/** @author Zach Kao */
	public T PositionGrid(string label, T initialSelection, int itemsPerCol)
	{
		T originalSelectedValue = _selectedValue;
		_selectedValue = initialSelection;
		bool anyChecked = false;
		
		EditorGUILayout.BeginHorizontal();
		for (int r = 0; r < _values.Length; r += itemsPerCol)
		{
			if(r == (_values.Length / 2))
			{
				EditorGUILayout.BeginVertical(GUILayout.Width(50));
				EditorGUILayout.Space();
				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.BeginVertical(GUILayout.Width(20));
			for (int i = r; i < r + itemsPerCol && i < _values.Length; i++)
			{
				if (_values[i].CompareTo(initialSelection) == 0) originalSelectedValue = initialSelection;
				if (GUILayout.Toggle(_values[i].CompareTo(_selectedValue) == 0, ""))
				{
					_selectedValue = _values[i];
					anyChecked = true;
				}
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndHorizontal();

		if (!anyChecked) _selectedValue = originalSelectedValue;
		return _selectedValue;
	}

	public SelectionList(T[] values, string[] labels)
	{
		_values = new T[values.Length];
		_labels = new string[labels.Length < values.Length ? values.Length : labels.Length];
		for (int i = 0; i < _values.Length; i++) _values[i] = values[i];
		for (int i = 0; i < _labels.Length; i++) _labels[i] = (i < labels.Length) ? labels[i] : values[i].ToString();
		_selectedValue = _values[0];
	}
}
