ModuleAnimateBeacon

This mod provides a simple module which lets you have two active animations working at the same time.
The basic module stanza is as follows (values shown are defaults)

	MODULE
	{
		name = ModuleAnimateDualGenerics

		moduleType = Beacon

		activateActionName = Turn On <<1>>
		stopActionName = Turn Off <<1>>
		actionGUIName = Turn On <<1>>
		animSpeed = 1
		repeating = true

        // Following do not have defaults

		anim_1_name = Glow
		anim_2_name = Rotate

		defaultActionGroup = Light
   	}

The last three values do not have default values.

The moduleType is only used as a textual name of the animation, which is put into the action prompts.  For example:

		moduleType = Beacon
		deployActionName = Turn On <<1>>

would create the following in the PAW:

        Turn On Beacon

which, by the way, shows you that the <<1>> is used as a placeholder which is replaced by the moduleType.

The defaultActionGroup allows you to be able to have the module assigned to an existing action group.

defaultActionGroup can be one of the following:

	    None
        Stage
        Gear
        Light
        RCS
        SAS
        Brakes
        Abort
        Custom01 
        Custom02 
        Custom03 
        Custom04 
        Custom05 
        Custom06 
        Custom07 
        Custom08 
        Custom09 
        Custom10 