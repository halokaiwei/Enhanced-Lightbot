using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.UI
{
    using Controller;

    public class MainMenuClickHandler : MonoBehaviour
    {
        #region Methods
            public void OnLevelButton (int index)
            {
                LevelManager.Instance.LoadLevel(index);
            }

        public void LoadSceneAdditive(string sceneName)
        {
            SceneManager.LoadScene("Signed-in", LoadSceneMode.Additive);
        }
        #endregion
    }
}
