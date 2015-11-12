/**
 * The MIT License (MIT)
 *
 * Copyright (c) 2015 Dreambound Studios AB
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;


namespace Dreambound
{
	[AttributeUsage(AttributeTargets.Method)]
	public class CmdAttribute : System.Attribute
	{
		public string Name { get; set; }
		public string Description { get; set; }

		public CmdAttribute(string name, string description)
		{
			Name = name;
			Description = description;
		}
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class VarAttribute : System.Attribute
	{
		public string Name { get; set; }

		public VarAttribute(string name)
		{
			Name = name;
		}
	}

	public class Command
	{
		public string Description { get; private set; }
		private MethodInfo m_method = null;
		private WeakReference m_instance = null;

		public Command(string desc, MethodInfo method, object target)
		{
			Description = desc;
			m_method = method;
			m_instance = target == null ? null : new WeakReference(target);
		}

		public string Invoke(string[] args)
		{
			object target = m_instance == null ? null : m_instance.Target;

			if (target == null && !m_method.IsStatic)
				return "";

			return m_method.Invoke(target, new object[]{args}) as string;
		}

		public bool IsSameTarget(object target)
		{
			if (m_instance != null && m_instance.Target == target)
				return true;
			return false;
		}
	}

	public class Variable
	{
		protected FieldInfo m_field = null;
		protected WeakReference m_instance = null;

		public Variable(FieldInfo field, object target)
		{
			m_field = field;
			m_instance = target == null ? null : new WeakReference(target);
		}

		public virtual void Set(string value)
		{
			object target = m_instance == null ? null : m_instance.Target;
			m_field.SetValue(target, value);
		}

		public string Get()
		{
			object target = m_instance == null ? null : m_instance.Target;
			return m_field.GetValue(target).ToString();
		}
	}

	public class IntVariable : Variable
	{
		public IntVariable(FieldInfo field, object target) : base(field, target)
		{
		}

		public override void Set(string value)
		{
			int val;
			if (int.TryParse(value, out val))
			{
				object target = m_instance == null ? null : m_instance.Target;
				m_field.SetValue(target, val);
			}
		}
	}

	public class FloatVariable : Variable
	{
		public FloatVariable(FieldInfo field, object target) : base(field, target)
		{
		}

		public override void Set(string value)
		{
			float val;
			if (float.TryParse(value, out val))
			{
				object target = m_instance == null ? null : m_instance.Target;
				m_field.SetValue(target, val);
			}
		}
	}

	public class BoolVariable : Variable
	{
		public BoolVariable(FieldInfo field, object target) : base(field, target)
		{
		}

		public override void Set(string value)
		{
			bool val;
			if (bool.TryParse(value, out val))
			{
				object target = m_instance == null ? null : m_instance.Target;
				m_field.SetValue(target, val);
			}
		}
	}

	public class Commander
	{
		private Dictionary<string, Command> m_commands = new Dictionary<string, Command>();
		private Dictionary<string, Variable> m_variables = new Dictionary<string, Variable>();

		public Dictionary<string, Command> Commands { get { return m_commands; } }
		public Dictionary<string, Variable> Variables { get { return m_variables; } }


		public Commander()
		{
			InspectAssembly();
			RegisterObject(this);
		}

		public string Execute(string line)
		{
			var args = line.Split(new Char[]{' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
			if (args == null || args.Length == 0)
				return "";

			// Perform variable substitution
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i].StartsWith("$"))
				{
					Variable variable;
					if (m_variables.TryGetValue(args[i], out variable))
						args[i] = variable.Get();
				}
			}

			// Execute command
			string result = "";
			Command cmd = null;

			if (m_commands.TryGetValue(args[0], out cmd))
				result = cmd.Invoke(args);

			return result;
		}

		public void PrintCommands()
		{
			//Debug.LogFormat("Command count {0}", m_commands.Count);

			foreach(var cmd in m_commands)
			{
				//Debug.Log(string.Format("{0} - {1}", cmd.Key, cmd.Value.Description));
			}
		}

		public void RegisterObject(object target)
		{
			InspectObject(target);
		}

		public void UnregisterObject(object target)
		{
			Dictionary<string, Command> newCommands = new Dictionary<string, Command>();

			foreach (var pair in m_commands)
			{
				Command cmd = pair.Value;
				if (!cmd.IsSameTarget(target))
					newCommands.Add(pair.Key, cmd);
			}

			m_commands = newCommands;
		}

		private void InspectObject(object target)
		{
			var type = target.GetType();
			CollectMethods(type, target);
			CollectVariables(type, target);
		}

		private void InspectAssembly()
		{
			var assembly = Assembly.GetExecutingAssembly();
			foreach (var type in assembly.GetTypes())
			{
				if (type.IsClass && type.IsPublic)
				{
					CollectMethods(type, null);
					CollectVariables(type, null);
				}
			}
		}

		private void CollectMethods(Type type, object target)
		{
			foreach (var method in type.GetMethods())
			{
				if (method.IsStatic || target != null)
				{
					CmdAttribute cmd = GetAttributeFromType(method, typeof(CmdAttribute)) as CmdAttribute;
					if (cmd != null)
					{
						if (!m_commands.ContainsKey(cmd.Name))
							m_commands.Add(cmd.Name, new Command(cmd.Description, method, target));
					}
				}
			}
		}

		private void CollectVariables(Type type, object target)
		{
			foreach (var field in type.GetFields())
			{
				if (field.IsStatic || target != null)
				{
					VarAttribute variable = GetAttributeFromType(field, typeof(VarAttribute)) as VarAttribute;
					if (variable != null)
					{
						if (!m_variables.ContainsKey(variable.Name))
						{
							var fieldType = field.FieldType;

							if (fieldType == typeof(string))
								m_variables.Add("$" + variable.Name, new Variable(field, target));
							else if (fieldType == typeof(int))
								m_variables.Add("$" + variable.Name, new IntVariable(field, target));
							else if (fieldType == typeof(float))
								m_variables.Add("$" + variable.Name, new FloatVariable(field, target));
							else if (fieldType == typeof(bool))
								m_variables.Add("$" + variable.Name, new BoolVariable(field, target));
						}
					}
				}
			}
		}

		private object GetAttributeFromType(MemberInfo member, Type attributeType)
		{
			object[] attributes = member.GetCustomAttributes(attributeType, true);
			if (attributes != null && attributes.Length > 0)
				return attributes[0];

			return null;
		}

		[Cmd("set", "Set the value of a particular variable")]
		public string SetVarCmd(string[] args)
		{
			if (args.Length != 3)
				return "";

			Variable variable;
			if (m_variables.TryGetValue(args[1], out variable))
			{
				variable.Set(args[2]);
				return args[2];
			}

			return "";
		}

		[Cmd("log", "Output to the log")]
		public static string LogCmd(string[] args)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 1; i < args.Length; i++)
				sb.Append(args[i]).Append(" ");

			Log.Info(sb.ToString());
			return "";
		}
	}
}
