using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private void Awake()
    {
        Registry.AddRegister<AudioManager>("manager_audio", this);
    }

    public void PlayClipAtPosition(Vector3 position)
    {

    }
}
