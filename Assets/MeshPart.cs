using UnityEngine;

public enum MeshType
{
    Spoiler,
    Body,
    WheelBLeft,
    WheelBRight,
    WheelFLeft,
    WheelFRight,
}

public class MeshPart : MonoBehaviour
{
    [SerializeField] private MeshType type;

    public MeshType MeshType => type;

    public Rigidbody Rigidbody
    {
        get; 
        private set;
    }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }
}
