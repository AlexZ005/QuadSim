using UnityEngine;
using System.Collections;

public class Console : MonoBehaviour {

	private Backpropagation bp = null;
	private quadPhysics qp = null;
	// Use this for initialization
    void Start () {
		bp = GameObject.Find("GM").GetComponent<Backpropagation>();
		qp = GameObject.Find("quadcopter").GetComponent<quadPhysics>();
        var repo = ConsoleCommandsRepository.Instance;
		repo.RegisterCommand("sr", ShowResults);
		repo.RegisterCommand("help", Help);
        repo.RegisterCommand("save", Save);
        repo.RegisterCommand("load", Load);
    }
	
	public string Help(params string[] args) {
        return "save [filename]\t\t save the map\n" +
            "load [filename]\t\t load the map\n" +
            "help\t\t\t show this command\n\n" +
			"sr\t\t\t ShowResults";
    }

    public string Save(params string[] args) {
        var filename = args[0];
        //new LevelSaver().Save(filename);
        return "Saved to " + filename;
    }

    public string Load(params string[] args) {
        var filename = args[0];
        //new LevelLoader().Load(filename);
        return "Loaded " + filename;
    }
	
	public string ShowResults(params string[] args) {
        var filename = args[0];
		float[] result = new float[4]; 
		
		ConsoleLog.Instance.Log("Instance [0] " + qp.transform.rotation[0]) ;
		ConsoleLog.Instance.Log("Instance [1] " + qp.transform.rotation[1]);
		ConsoleLog.Instance.Log("Instance [2] " + qp.transform.rotation[2]);
		ConsoleLog.Instance.Log("Instance [3] " + qp.transform.rotation[3]);
		
		result = bp.feedForwardContinue(qp.transform.rotation[0],qp.transform.rotation[1],qp.transform.rotation[2],qp.transform.rotation[3]);
		
        //new LevelLoader().Load(filename);
        return "\n\nLoaded " + result[0] + "\n[1] " + result[1] + "\n[2] " + result[2] + "\n[3] " + result[3]; //взять следующие данные, которые посчитала нейронная сеть
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
