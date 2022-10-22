using Reports;
using Structures;
using UnityEngine;
using static Managers.GameManager;

namespace Platform
{
    public enum PlatformID
    {
        Base,
        Steam
    }
    
    public class PlatformDelegate
    {
        protected virtual PlatformID GetPlatformId()
        {
            return PlatformID.Base;
        }
    }

    public class AchievementsDelegate : PlatformDelegate
    {
        public virtual void Initialise()
        {
            Debug.LogError($"Call to unimplemented {GetPlatformId().ToString()} " +
                                       "delegate method AchievementsDelegate.Initialise");
        }
        
        public virtual void UnlockAchievement(Achievement achievement)
        {
            Debug.LogError($"Call to unimplemented {GetPlatformId().ToString()} " +
                                       "delegate method AchievementsDelegate.UnlockAchievement");
        }
        
        public virtual void UpdateStat(Milestone stat, int value)
        {
            Debug.LogError($"Call to unimplemented {GetPlatformId().ToString()} " +
                                       "delegate method AchievementsDelegate.UpdateStat");
        }
        
        public virtual void UpdateProgress(Milestone stat, Achievement achievement, int value)
        {
            Debug.LogError($"Call to unimplemented {GetPlatformId().ToString()} " +
                                       "delegate method AchievementsDelegate.UpdateProgress");
        }
        
        public virtual void ResetAll()
        {
            Debug.LogError($"Call to unimplemented {GetPlatformId().ToString()} " +
                                       "delegate method AchievementsDelegate.ResetAll");
        }
    }
    
    public class InputDelegate : PlatformDelegate
    {
        public virtual int GetDefaultControlScheme()
        {
            return 0;
        }
    }
    
    public class FileSystemDelegate : PlatformDelegate
    {
        public Managers.SaveFile SaveFile;

        public FileSystemDelegate()
        {
            SaveFile = new Managers.SaveFile();
        }

        public virtual string GetSaveFilePath()
        {
            return Application.persistentDataPath + "/save.json";
        }
        public virtual string GetBackupFilePath()
        {
            return Application.persistentDataPath + "/save_prev.json";
        }
    }
    
    public class GameplayDelegate : PlatformDelegate
    {
        public bool GenerateColliders = true;

        public virtual PlatformAssets GetPlatformAssets()
        {
            return Resources.Load<PlatformAssets>("DefaultPlatformAssets");
        }

        public virtual Structure GetHoveredStructure(Camera camera, LayerMask collisionMask)
        {
            var hit = Manager.Inputs.GetRaycast(camera, 200f, collisionMask);
            return hit.collider ? hit.collider.GetComponentInParent<Structure>() : null;
        }

        public virtual bool IsOverUI(UnityEngine.EventSystems.EventSystem eventSystem)
        {
            return eventSystem.IsPointerOverGameObject();
        }
    }
}
