using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;

public class UnstableCoreTests
{
    private static readonly List<ItemType> Types = new()
    {
        ItemType.Booster, ItemType.GravityBoots, ItemType.Ammo 
    };

    [UnityTest]
    public IEnumerator ItemSuccessfulyAdded([ValueSource(nameof(Types))] ItemType type)
    {
        yield return LoadGame();
        
        Inventory.Instance.AddItem(type, 1);
        yield return null;
        Assert.IsTrue(Inventory.Instance.CanUseItem(type));
        yield return null;
    }

    [UnityTest]
    public IEnumerator ItemSuccessfulyRemovedAfterUse([ValueSource(nameof(Types))] ItemType type)
    {
        yield return LoadGame();

        Inventory.Instance.AddItem(type, 1);
        Inventory.Instance.UseItem(type);
        Assert.IsFalse(Inventory.Instance.CanUseItem(type));
        yield return null;
    }



    private IEnumerator LoadGame()
    {
        yield return Addressables.LoadSceneAsync("Assets/Scenes/Managers.unity", UnityEngine.SceneManagement.LoadSceneMode.Single);
        yield return Addressables.LoadSceneAsync("Assets/Scenes/MainScene.unity", UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }
}
