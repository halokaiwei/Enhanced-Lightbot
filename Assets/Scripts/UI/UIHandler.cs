    using UnityEngine;
    using UnityEngine.UI;

    namespace Game.UI
    {
        using Controller;

        public class UIHandler : MonoBehaviour
        {
            #region Variables
                [SerializeField]
                private Transform mainProc;

                [SerializeField]
                private Transform[] procs;

                [SerializeField]
                private GameObject buttonRun;

                [SerializeField]
                private GameObject buttonReset;

                [SerializeField]
                private Color colorProcActive;

                [SerializeField]
                private Color colorProcDeactive;

                [SerializeField]
                private GameObject panelFinish;

                public static UIHandler Instance;
                public Transform activeProc;

                public int movementNumber;
            #endregion

            #region Methods
                private void Awake ()
                {
                    if (Instance != null)
                        Destroy(this.gameObject);
                    Instance = this;
                    activeProc = mainProc;
                    
                }

                private void Start() {
                        movementNumber=0;
                }

                internal void AddOperation (UIOperation uiOperation)
                {
                    UIOperation instance = Instantiate(uiOperation
                        , Vector3.zero, Quaternion.identity, activeProc);
                    instance.IsAdded = true;

                    if (activeProc == mainProc) {
                        movementNumber++;
                        GameManager.Instance.AddOperation(instance.Operation);
                    }
                    else
                        GameManager.Instance.AddOperationInSubProcedure(instance.Operation, GetIndex(activeProc));
                }

                internal void RemoveOperation (UIOperation uiOperation)
                {
                    Destroy(uiOperation.gameObject);

                    if (activeProc == mainProc) {
                        movementNumber--;
                        GameManager.Instance.RemoveOperation(uiOperation.Operation);
                    }
                    else
                        GameManager.Instance.RemoveOperationFromSubProcedure(uiOperation.Operation, GetIndex(activeProc));
                }

                public void Run ()
                {
                    Debug.Log("movementNumber:"+movementNumber);
                    GameManager.Instance.RunCode();
                    ShowHideRunButton(false);
                }

                public void Reset ()
                {
                    GameManager.Instance.ResetCode();
                    ShowHideRunButton(true);
                }

                private void ShowHideRunButton (bool isShow)
                {
                    buttonRun.SetActive(isShow);
                    buttonReset.SetActive(!isShow);
                }

                public void OnProc (Transform caller)
                {
                    DeactiveAllProcs();
                    caller.parent.parent.parent.GetComponent<Image>().color = colorProcActive;
                    activeProc = caller;
                }

                private void DeactiveAllProcs ()
                {
                    mainProc.parent.parent.parent.GetComponent<Image>().color =  colorProcDeactive;

                    foreach (Transform proc in procs)
                    {
                        proc.parent.parent.parent.GetComponent<Image>().color = colorProcDeactive;
                    }
                }

                private int GetIndex (Transform proc)
                {
                    for (int i = 0; i < procs.Length; i++)
                    {
                        if (proc == procs[i])
                            return i;
                    }

                    return -1;
                }

                public void ShowFinish ()
                {
                    panelFinish.SetActive(true);
                }

                public void NextLevel ()
                {
                    LevelManager.Instance.LoadNextLevel();
                }

                public void BackToMain ()
                {
                    LevelManager.Instance.LoadLevel(0);
                }
            #endregion
        }
    }