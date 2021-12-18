#if UNITY_IPHONE || UNITY_IOS || UNITY_TVOS
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;


public class EntitlementsPostProcess : ScriptableObject {


    [PostProcessBuild(999)]
    public static void OnPostProcess(BuildTarget buildTarget, string buildPath) {
        

        //write capabilities
        string projPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";

        PBXProject proj = new PBXProject();
        proj.ReadFromString(File.ReadAllText(projPath));

        string target = proj.TargetGuidByName("Unity-iPhone");

        proj.AddCapability(target, PBXCapabilityType.InAppPurchase);

        File.WriteAllText(projPath, proj.WriteToString());


        //write plist file
        string plistPath = Path.Combine(buildPath, "Info.plist");

        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        Tr.Instance.updateLanguage(SystemLanguage.English);
        plist.root.SetString("NSPhotoLibraryUsageDescription", Tr.get("S1.Authorization.IOS"));
        plist.root.SetString("NSPhotoLibraryAddUsageDescription", Tr.get("S1.Authorization.IOS"));

        File.WriteAllText(plistPath, plist.WriteToString());
    }

}
#endif
