using UnityEngine;

public class SelectLevelButton : MonoBehaviour {
    public int level { get; set; }
    public void Click()
    {
        FindObjectOfType<GenerateButtonBasedUserRate>().ButtonClick(level);
    }
}
