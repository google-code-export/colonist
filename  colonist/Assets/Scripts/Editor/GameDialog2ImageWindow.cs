using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Linq;
using System.Xml;
using System.Collections.Generic;

public class GameDialog2ImageWindow : EditorWindow
{
	[MenuItem("Window/GameDialogue2Image")]	
	public static void init ()
	{
		GameDialog2ImageWindow window = (GameDialog2ImageWindow)EditorWindow.GetWindow (typeof(GameDialog2ImageWindow)); 
	}
	
	int Height = 64;
	int Width = 512;

//	static readonly string[] fonts = GetAllFontFamilies ();
//	static readonly string[] languages = Localization.SupportLanguage.Split (new[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
	const string HTML2ImagePath = "/ExternalExecutable/html2img.exe";
	TextAsset XMLTextToBeConverted = null;
	bool OnlyGenerateOnImage = false;
	string TargetImageID = "";

	void OnGUI ()
	{
		var dialogFiles = Resources.LoadAll (Localization.DialogAssetRootFolder, typeof(TextAsset));
		    
		XMLTextToBeConverted = (TextAsset)EditorGUILayout.ObjectField ("Dialog XML textasset:", XMLTextToBeConverted, typeof(TextAsset));
		if (OnlyGenerateOnImage = EditorGUILayout.Toggle ("Has target image:", OnlyGenerateOnImage)) {
			TargetImageID = EditorGUILayout.TextField ("Target image ID:", TargetImageID);
		}
		    
		EditorGUILayout.BeginHorizontal ();
		Height = EditorGUILayout.IntField (new GUIContent ("Height:", "Height of the output image, recommand 4:3(width:height)"), Height);
		Width = EditorGUILayout.IntField (new GUIContent ("Width:", "Width of the output image, recommand 4:3(width:height)"), Width);
		EditorGUILayout.EndHorizontal ();
		
		if (GUILayout.Button ("Generate")) {

			string assetPath = AssetDatabase.GetAssetPath (XMLTextToBeConverted);
			StringReader stringReader = new StringReader (XMLTextToBeConverted.text);
			// read the select text asset into a XMLDocument object:
			XmlDocument xml = new XmlDocument ();
			xml.Load (stringReader);
			stringReader.Close ();
			var folder = Path.GetDirectoryName (assetPath);
			foreach (XmlElement dialog in xml.DocumentElement.GetElementsByTagName("dialog")) {
				foreach (XmlElement item in dialog.GetElementsByTagName("dialogitem")) {
					if (OnlyGenerateOnImage && TargetImageID.Contains (item.GetAttribute ("image")) == false) {
					    continue;	      
					}
					var path = string.Concat (folder, "/", item.GetAttribute ("image"), ".tga");
					var content = item.InnerText;
					ConvertContentToTga (content, path);
				}
			}


			AssetDatabase.Refresh ();

			foreach (var item in Resources.LoadAll(Localization.DialogAssetRootFolder, typeof(Texture2D)))
				SetTextureSettings (item);

			Debug.Log ("generated");
		}

		EditorGUILayout.Space ();
		if (GUILayout.Button ("Select all textures at dialog folder")) {
			Selection.objects = Resources.LoadAll (Localization.DialogAssetRootFolder, typeof(Texture2D));
		}


		EditorGUILayout.Space ();
		if (GUILayout.Button ("Conevrt xml to version2")) {
			foreach (TextAsset asset in dialogFiles) {
				var path = AssetDatabase.GetAssetPath (asset);
				ConertToNewVersion (path, path);
			}
			Debug.Log ("converted");
		}
		 
	}

	private static string[] GetAllFontFamilies ()
	{
		var startInfo = new System.Diagnostics.ProcessStartInfo ()
        {
            FileName = Application.dataPath + HTML2ImagePath,
            Arguments = "FONTS",
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            StandardOutputEncoding = System.Text.Encoding.Default //for viewing in inspector
        };
		return System.Diagnostics.Process.Start (startInfo).StandardOutput.ReadToEnd ().Split (new[] { System.Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries);
	}

	private static void ConertToNewVersion (string input, string output)
	{
		var language = System.IO.Path.GetFileNameWithoutExtension (input);
		XmlDocument doc = new XmlDocument ();
		doc.Load (input);

		foreach (XmlElement dialog in doc.DocumentElement.GetElementsByTagName("dialog")) {//dialog
			var id = dialog.GetAttribute ("id");
			int index = 0;
			foreach (XmlElement dialogitem in dialog.GetElementsByTagName("dialogitem")) {
				if (!dialogitem.HasAttribute ("text")) {
					Debug.Log ("It's been new version");
					return;
				}
				var text = dialogitem.GetAttribute ("text");
				dialogitem.RemoveAttribute ("text");

				dialogitem.InnerText = text;

				var attr = doc.CreateAttribute ("image");
				attr.Value = string.Format ("{0}_dialog{1}_{2}", language, id, index);
				dialogitem.Attributes.Append (attr);
				index++;
			}
		}
		doc.Save (output);
	}

	void ConvertContentToTga (string content, string tga, bool recreate = true)
	{
		if (string.IsNullOrEmpty (content)) {
			return;
		}
		if (File.Exists (tga) && !recreate) {
			return;
		}

		var startInfo = new System.Diagnostics.ProcessStartInfo ()
        {
            FileName = Application.dataPath + HTML2ImagePath,
            Arguments = string.Format (@"{0} {1} {2} ""{3}""", Width, Height, tga, content),
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true
        };
		System.Diagnostics.Process.Start (startInfo).WaitForExit (20000);
	}

    #region Change Texture Settings
	/// <summary>
	/// change textureType to GUI
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	public TextureImporter SetTextureSettings (string path)
	{
		TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath (path);
		textureImporter.filterMode = FilterMode.Point;
		textureImporter.textureType = TextureImporterType.GUI;
		AssetDatabase.ImportAsset (path);
		return textureImporter;
	}

	/// <summary>
	/// change textureType to GUI
	/// </summary>
	public TextureImporter SetTextureSettings (Object o)
	{
		return SetTextureSettings (AssetDatabase.GetAssetPath (o));
	}

    #endregion
}