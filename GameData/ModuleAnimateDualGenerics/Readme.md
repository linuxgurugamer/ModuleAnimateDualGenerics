ModuleAnimateBeacon

This mod provides a simple module which lets you have two active animations in the same part working at the same time.
The basic module stanza is as follows (values shown are defaults)

	MODULE
	{
		name = ModuleAnimateDualGenerics

		moduleType = Beacon

        // Activate and stop both animations

		activateActionName = Turn On <<1>>
		stopActionName = Turn Off <<1>>

        // Activate and stop the first animation

        activateAnim_1_Name = 
        stopAnim_1_Name = 

        // Activate and stop the second animation

        activateAnim_2_Name = 
        stopAnim_2_Name = 

		actionGUIName = Turn On <<1>>
		animSpeed = 1
		repeating = true

        toggleActionName = Toggle <<1>>

        // Following do not have defaults

		anim_1_name = Glow
		anim_2_name = Rotate

        // Default action group , acts as a toggle
		defaultActionGroup = Light

   	}

The last three values do not have default values.

The module gives you the ability to control both animations at the same time, or individually.  There are three sets
of names, grouped together above.  If no value is given to any of the values, no button for that action will be
added to the PAW.

The toggle action works as follows:
    If no animation is running, turn on both animations
    If either or both of the animations are running, then turn them all off

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