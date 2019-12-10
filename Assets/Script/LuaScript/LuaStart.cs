using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaStart : MonoBehaviour {
    #region script
    string script = @"
    xlua.hotfix(CS.BubbleScript, 'UpdateBubble', 
    function(self, deltaTime)
        --print('LuaUpdateBubble')
        local bubble = self
		local targetMapTilePos = bubble.TargetMapTilePos
        local targetVec = targetMapTilePos - bubble.transform.localPosition
        --local moveVec = (targetVec.normalized * bubble.DropSpeed) * deltaTime
        local moveVec = (targetVec.normalized * CS.BubbleScript.DropSpeed) * deltaTime
        
        local bubbleTrans = bubble.transform

	    if moveVec.magnitude >= targetVec.magnitude then
		    local nextTile = bubble.TargetMapTile:NextTile()
			if nextTile == nil then
				bubbleTrans.localPosition =  targetMapTilePos
				return false;
			else
				bubbleTrans.localPosition = targetMapTilePos + (nextTile.transform.localPosition - targetMapTilePos).normalized * (moveVec.magnitude - targetVec.magnitude)
				bubble.TargetMapTile.TargetBubble = nil
				bubble.TargetMapTile = nextTile
				nextTile.TargetBubble = bubble
				return true
			end
		else
			bubbleTrans.localPosition = bubbleTrans.localPosition + moveVec
	    end

		return true


        --print( 'targetVec', targetVec:ToString(),  'moveVec', moveVec:ToString())
        --print( 'TestLua', deltaTime, 'TargetVec', targetVec:ToString(), multi )
    end)
    ";
    #endregion

    private void Start()
    {
        LuaEnv luaenv = new LuaEnv();
        Run(luaenv);
        luaenv.Dispose();

    }

    void Run(LuaEnv luaenv)
    {
        luaenv.DoString("require 'main'");
    }

}
