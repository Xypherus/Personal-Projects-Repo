using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class CustomYarnVariableStorage : Yarn.Unity.VariableStorageBehaviour, IEnumerable<KeyValuePair<string, Yarn.Value>>
{

    /// Where we actually keeping our variables
    private Dictionary<string, Yarn.Value> variables = new Dictionary<string, Yarn.Value>();

    /// <summary>
    /// A default value to apply when the object wakes up, or when
    /// ResetToDefaults is called.
    /// </summary>
    [System.Serializable]
    public class DefaultVariable
    {
        /// <summary>
        /// The name of the variable.
        /// </summary>
        /// <remarks>
        /// Do not include the `$` prefix in front of the variable
        /// name. It will be added for you.
        /// </remarks>
        public string name;

        /// <summary>
        /// The value of the variable, as a string.
        /// </summary>
        /// <remarks>
        /// This string will be converted to the appropriate type,
        /// depending on the value of <see cref="type"/>.
        /// </remarks>
        public string value;

        /// <summary>
        /// The type of the variable.
        /// </summary>
        public Yarn.Value.Type type;
    }

    /// <summary>
    /// The list of default variables that should be present in the
    /// InMemoryVariableStorage when the scene loads.
    /// </summary>
    public DefaultVariable[] defaultVariables;

    [Header("Optional debugging tools")]

    /// A UI.Text that can show the current list of all variables. Optional.
    [SerializeField]
    internal UnityEngine.UI.Text debugTextView = null;

    private bool initilized = false;

    /// Reset to our default values when the game starts
    internal void Awake()
    {
        Debug.Log("Variable Storage Initilized");

        ResetToDefaults();
    }

    /// <summary>
    /// Removes all variables, and replaces them with the variables
    /// defined in <see cref="defaultVariables"/>.
    /// </summary>
    public override void ResetToDefaults()
    {
        Clear();

        // For each default variable that's been defined, parse the
        // string that the user typed in in Unity and store the
        // variable
        foreach (var variable in defaultVariables)
        {

            object value;

            switch (variable.type)
            {
                case Yarn.Value.Type.Number:
                    float f = 0.0f;
                    float.TryParse(variable.value, out f);
                    value = f;
                    break;

                case Yarn.Value.Type.String:
                    value = variable.value;
                    break;

                case Yarn.Value.Type.Bool:
                    bool b = false;
                    bool.TryParse(variable.value, out b);
                    value = b;
                    break;

                case Yarn.Value.Type.Variable:
                    // We don't support assigning default variables from
                    // other variables yet
                    Debug.LogErrorFormat("Can't set variable {0} to {1}: You can't " +
                        "set a default variable to be another variable, because it " +
                        "may not have been initialised yet.", variable.name, variable.value);
                    continue;

                case Yarn.Value.Type.Null:
                    value = null;
                    break;

                default:
                    throw new System.ArgumentOutOfRangeException();

            }

            var v = new Yarn.Value(value);

            SetValue("$" + variable.name, v);
        }

        initilized = false;
        foreach (var global in FlagManager.FlagManagerInstance.GlobalVariables)
        {
            Debug.Log("Adding Global Variable " + global.Key);
            var v = new Yarn.Value(global.Value);
            SetValue(global.Key, v);
        }
        foreach (var inTime in FlagManager.FlagManagerInstance.InTimeVariables)
        {
            Debug.Log("Adding Local Variable " + inTime.Key);
            var v = new Yarn.Value(inTime.Value);
            SetValue(inTime.Key, v);
        }
        initilized = true;

        FlagManager.FlagManagerInstance.updateJournalText();
        Debug.Log("Variables Length = " + variables.Count);
    }

    /// <summary>
    /// Stores a <see cref="Value"/>.
    /// </summary>
    /// <param name="variableName">The name to associate with this
    /// variable.</param>
    /// <param name="value">The value to store.</param>
    public override void SetValue(string variableName, Yarn.Value value)
    {
        // Copy this value into our list
        variables[variableName] = new Yarn.Value(value);


        if (FlagManager.FlagManagerInstance != null)
        {
            //Debug.Log("Attempting to edit Global Variable " + variableName);
            if (FlagManager.FlagManagerInstance.GlobalVariables.ContainsKey(variableName) && value.type == Yarn.Value.Type.Bool && initilized)
            {
                Debug.Log("Editing Global Variable " + variableName);
                FlagManager.FlagManagerInstance.GlobalVariables[variableName] = value.AsBool;
                FlagManager.FlagManagerInstance.updateJournalText();
            }
            else if (value.type == Yarn.Value.Type.Bool && initilized)
            {
                if (FlagManager.FlagManagerInstance.InTimeVariables.ContainsKey(variableName))
                {
                    Debug.Log("Editing InTime Variable " + variableName);
                    FlagManager.FlagManagerInstance.InTimeVariables[variableName] = value.AsBool;
                }
                else
                {
                    Debug.Log("Adding InTime Variable " + variableName);
                    FlagManager.FlagManagerInstance.InTimeVariables.Add(variableName, value.AsBool);
                }
            }
        }
    }

    /// <summary>
    /// Retrieves a <see cref="Value"/> by name.
    /// </summary>
    /// <param name="variableName">The name of the variable to retrieve
    /// the value of.</param>
    /// <returns>The <see cref="Value"/>. If a variable by the name of
    /// <paramref name="variableName"/> is not present, returns a value
    /// representing `null`.</returns>
    public override Yarn.Value GetValue(string variableName)
    {
        // If we don't have a variable with this name, return the null
        // value
        if (variables.ContainsKey(variableName) == false)
            return Yarn.Value.NULL;

        return variables[variableName];
    }

    /// <summary>
    /// Removes all variables from storage.
    /// </summary>
    public override void Clear()
    {
        variables.Clear();
    }

    /// If we have a debug view, show the list of all variables in it
    internal void Update()
    {
        if (debugTextView != null)
        {
            var stringBuilder = new System.Text.StringBuilder();
            foreach (KeyValuePair<string, Yarn.Value> item in variables)
            {
                //Debug.Log("Updating Value for " + item.Key);

                string debugDescription;
                switch (item.Value.type)
                {
                    case Yarn.Value.Type.Bool:
                        debugDescription = item.Value.AsBool.ToString();
                        break;
                    case Yarn.Value.Type.Null:
                        debugDescription = "null";
                        break;
                    case Yarn.Value.Type.Number:
                        debugDescription = item.Value.AsNumber.ToString();
                        break;
                    case Yarn.Value.Type.String:
                        debugDescription = $@"""{item.Value.AsString}""";
                        break;
                    default:
                        debugDescription = "<unknown>";
                        break;

                }
                stringBuilder.AppendLine(string.Format("{0} = {1}",
                                                        item.Key,
                                                        debugDescription));
            }
            debugTextView.text = stringBuilder.ToString();
            debugTextView.SetAllDirty();
        }
    }

    /// <summary>
    /// Returns an <see cref="IEnumerator{T}"/> that iterates over all
    /// variables in this object.
    /// </summary>
    /// <returns>An iterator over the variables.</returns>
    IEnumerator<KeyValuePair<string, Yarn.Value>> IEnumerable<KeyValuePair<string, Yarn.Value>>.GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<string, Yarn.Value>>)variables).GetEnumerator();
    }

    /// <summary>
    /// Returns an <see cref="IEnumerator"/> that iterates over all
    /// variables in this object.
    /// </summary>
    /// <returns>An iterator over the variables.</returns>        
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<string, Yarn.Value>>)variables).GetEnumerator();
    }
}

