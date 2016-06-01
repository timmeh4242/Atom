namespace ExternalTools
{
    using System.IO;
    using UnityEditor;

    [InitializeOnLoad]
    public static class Atom
    {
        public static string AtomPath = "/usr/local/bin/atom";

        static string ProjectPath
        {
            get
            {
                return System.IO.Path.GetDirectoryName(UnityEngine.Application.dataPath);
            }
        }

        static void CallExternalEditor(string args)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();

#if UNITY_EDITOR_OSX
            proc.StartInfo.FileName = AtomPath;
            proc.StartInfo.Arguments = args;
            proc.StartInfo.UseShellExecute = false;
#else
            return;
#endif
            proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();
        }

        [UnityEditor.Callbacks.OnOpenAssetAttribute()]
        static bool OnOpenedAsset(int instanceID, int line)
        {
            // determine asset that has been double clicked in the project view
            UnityEngine.Object selected = EditorUtility.InstanceIDToObject(instanceID);

            if (selected.GetType().ToString() == "UnityEditor.MonoScript" ||
                selected.GetType().ToString() == "UnityEngine.Shader")
            {
                string completeFilepath = ProjectPath + Path.DirectorySeparatorChar + AssetDatabase.GetAssetPath(selected);
                string args = null;
                if (line == -1)
                {
                    args = completeFilepath;
                }
                else
                {
                    args = completeFilepath + ":" + line.ToString();
                }

                CallExternalEditor(args);
                return true;
            }

            // didn't find a code file? let unity figure it out
            return false;
        }
    }
}
