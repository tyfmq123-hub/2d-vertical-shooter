using UnityEngine;
using UnityEngine.UI;

public class Test3Main : MonoBehaviour
{
    public Button btn;
   
    public TestEnemy enemyAGo;
    
    void Start()
    {
        enemyAGo.onDie = (pos) =>
        {
            TestItem.Instance.CreateItem(pos);
            enemyAGo = null;
        };

        btn.onClick.AddListener(() =>
        {
            Debug.Log("clicked!!");
            enemyAGo.TakeDamage(5);
        });    
    }
}