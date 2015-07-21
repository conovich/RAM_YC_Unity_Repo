using UnityEngine;
using System.Collections;

public interface ILoggable {

	//SHOULD CALL THIS ON FIXEDUPDATE(), not Update() FOR FRAMERATE INDEPENDENCE
	void Log();

}
