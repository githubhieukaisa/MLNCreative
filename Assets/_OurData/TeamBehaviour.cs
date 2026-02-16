using UnityEngine;

public class TeamBehaviour : MonoBehaviour
{
    protected virtual void Awake()
    {
        this.LoadComponents();
        this.ResetValue();
    }
    private void Reset()
    {
        this.LoadComponents();
        this.ResetValue();
    }

    [ContextMenu("Load Components Only")]
    protected void LoadComponentsContextMenu()
    {
        this.LoadComponents();
    }

    protected virtual void LoadComponents()
    {
        //todo load component
    }

    protected virtual void ResetValue()
    {
        // todo reset value
    }

}
