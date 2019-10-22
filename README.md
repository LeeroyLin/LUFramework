# LUFramework
A unity framework. mvc, table transformer, UIManager, LogManager, NotificationManager...

Q:
# How to change the file header's info.
A:
	1. Open the config class in "Frameworks/LUFramework/Config/Config.cs".
	2. Change NAME_SPACE, PROJ_NAME and DEVELOPER.


Q:
# How to change the template.
A:
	You can change the template in "Frameworks/LUFramework/Template".


Q: 
# How to tranform a excel file to a class.
A: 
	1. Click menu item "LUFramework" - "TableTransformer" - "Transform".
	2. Click "+" button to choose your excel files folder path and your target classes path.
	3. Click "Start Transform" button to transform your table files.
	Note: The extension of excel files should be ".xlsx".
	Note: The format of the excel file should be same with the files in "Example/TableTransformer/SourceTables".


Q:
# How to create a template script.
A:
	1. Right click Assets.
	2. Choose "Create" - "LUFramework".
	3. Choose the script you want to create.


Q:
# How to create view and control scripts automatically.
A:
	1. Drag "Resources/LUFramework/LUFCanvas" to Hierarchy. This is the ui canvas.
	2. Create a panel as a form. you can add any ui component to it.
	3. If you want to export the ui, rename it with "_".
	4. Save this form as a prefab, and put it into "Resources/LUFramework/Forms".
	5. Right click this form, choose "LUFramework" - "AutoCreateMVC".
	6. Hold on and there will be some scripts in your scripts folder.
	7. The view script has got the export ui components, you can use them directly.
	8. The view script has binded the event to notification center, you can write the logic in controller script directly.
	9. Apply this form prefab.
	10. Remove this form object from LUFCanvas.
	11. You can remove LUFCanvas too, it's not necessary.


Q:
# How to show form.
A:
	1. UIManager.Instance.ShowForm("TestForm", EFormType.Normal, EFormDisplayType.Single, EFormModalType.Translucency);
	1.1. param1: Your form prefab's name.
	1.2. param2: Which node you want this form to create at.
	1.3. param3: Display type. "single" means only show this form. "Stack" means forms use stack to control.
	1.4. param4: Modal type. What's your form's background? Can you click under form?


Q:
# How to use pool.
A:
	1. PoolManager.Instance.TryGet("Tag", GetObj);
	1.1. param1: Pool item's tag.
	1.2. param2: Object creator function.
	1.3. param3: Init function.(Choosable)
	2. PoolManager.Instance.Recover("Tag", obj);
	2.1. param1: Pool item's tag.
	2.2. param2: The object you want to recover.


Q: 
# How to use notification center.
A:
	1. RegisterHandler("MsgTag", HandlerFunc);
	1.1. param1: Notification tag.
	1.2. param2: Handler function.
	2. SendNotification("MsgTag");
	2.1. param1: Notification tag.
