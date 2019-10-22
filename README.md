# LUFramework
A unity framework. mvc, table transformer, UIManager, LogManager, NotificationManager...

# How to change the file header's info.
A:</br>
	1. Open the config class in "Frameworks/LUFramework/Config/Config.cs".</br>
	2. Change NAME_SPACE, PROJ_NAME and DEVELOPER.


# How to change the template.
A:</br>
	You can change the template in "Frameworks/LUFramework/Template".


# How to tranform a excel file to a class.
A: </br>
	1. Click menu item "LUFramework" - "TableTransformer" - "Transform".</br>
	2. Click "+" button to choose your excel files folder path and your target classes path.</br>
	3. Click "Start Transform" button to transform your table files.</br>
	Note: The extension of excel files should be ".xlsx".</br>
	Note: The format of the excel file should be same with the files in "Example/TableTransformer/SourceTables".


# How to create a template script.
A:</br>
	1. Right click Assets.</br>
	2. Choose "Create" - "LUFramework".</br>
	3. Choose the script you want to create.


# How to create view and control scripts automatically.
A:</br>
	1. Drag "Resources/LUFramework/LUFCanvas" to Hierarchy. This is the ui canvas.</br>
	2. Create a panel as a form. you can add any ui component to it.</br>
	3. If you want to export the ui, rename it with "_".</br>
	4. Save this form as a prefab, and put it into "Resources/LUFramework/Forms".</br>
	5. Right click this form, choose "LUFramework" - "AutoCreateMVC".</br>
	6. Hold on and there will be some scripts in your scripts folder.</br>
	7. The view script has got the export ui components, you can use them directly.</br>
	8. The view script has binded the event to notification center, you can write the logic in controller script directly.</br>
	9. Apply this form prefab.</br>
	10. Remove this form object from LUFCanvas.</br>
	11. You can remove LUFCanvas too, it's not necessary.


# How to show form.
A:</br>
	1. UIManager.Instance.ShowForm("TestForm", EFormType.Normal, EFormDisplayType.Single, EFormModalType.Translucency);</br>
	1.1. param1: Your form prefab's name.</br>
	1.2. param2: Which node you want this form to create at.</br>
	1.3. param3: Display type. "single" means only show this form. "Stack" means forms use stack to control.</br>
	1.4. param4: Modal type. What's your form's background? Can you click under form?


# How to use pool.
A:</br>
	1. PoolManager.Instance.TryGet("Tag", GetObj);</br>
	1.1. param1: Pool item's tag.</br>
	1.2. param2: Object creator function.</br>
	1.3. param3: Init function.(Choosable)</br>
	2. PoolManager.Instance.Recover("Tag", obj);</br>
	2.1. param1: Pool item's tag.</br>
	2.2. param2: The object you want to recover.


# How to use notification center.
A:</br>
	1. RegisterHandler("MsgTag", HandlerFunc);</br>
	1.1. param1: Notification tag.</br>
	1.2. param2: Handler function.</br>
	2. SendNotification("MsgTag");</br>
	2.1. param1: Notification tag.
