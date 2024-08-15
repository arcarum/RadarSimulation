using System.Diagnostics;
using UnityEngine.UIElements;

public class CSVMenuUI
{
    VisualElement ui;
    MainMenuController mainMenuController;
    CSVManager csvManager;

    string fileName = "Scenario";

    public CSVMenuUI(VisualElement ui, MainMenuController mainMenuController, CSVManager csvManager)
    {
        this.ui = ui;
        this.mainMenuController = mainMenuController;
        this.csvManager = csvManager;
    }
    
    public void SetBtnEvents()
    {
        TextField fileNameTxtField = ui.Q("FileNameTxtField") as TextField;
        fileNameTxtField.RegisterValueChangedCallback(evt => {
            csvManager.fileName = evt.newValue;
        });

        SliderInt numOfShipsSlider = ui.Q("NumOfShipsSlider") as SliderInt;
        csvManager.numberOfShips = numOfShipsSlider.value;
        numOfShipsSlider.RegisterValueChangedCallback(evt => {
            csvManager.numberOfShips = evt.newValue;
        });

        SliderInt numOfLocationsSlider = ui.Q("NumOfLocationsSlider") as SliderInt;
        csvManager.locationsToCreate = numOfLocationsSlider.value;
        numOfLocationsSlider.RegisterValueChangedCallback(evt => {
            csvManager.locationsToCreate = evt.newValue;
        });

        Label minMaxLabel = ui.Q("MinMaxLabel") as Label;

        MinMaxSlider minMaxSlider = ui.Q("MinMaxSlider") as MinMaxSlider;
        csvManager.minStartingCoordinates = minMaxSlider.value.x;
        csvManager.maxStartingCoordinates = minMaxSlider.value.y;
        minMaxLabel.text = $"Coordinate Range:\nMin Value: {minMaxSlider.value.x}\nMax Value: {minMaxSlider.value.y}";
        minMaxSlider.RegisterValueChangedCallback(evt => {
            
            minMaxLabel.text = $"Coordinate Range:\nMin Value: {evt.newValue.x}\nMax Value: {evt.newValue.y}";

            csvManager.minStartingCoordinates = evt.newValue.x;
            csvManager.maxStartingCoordinates = evt.newValue.y;
        });

        Button generateRandomCSVBtn = ui.Q("GenerateCSVBtn") as Button;
        generateRandomCSVBtn.RegisterCallback((ClickEvent clickEvent) => {
            csvManager.generateRandomCSV = !csvManager.generateRandomCSV;
            mainMenuController.ResetScenarioDropdownField();
        });
    }

    public void SetFileNameTextFIeld(int numberOfNextScenario)
    {
        TextField fileNameTxtField = ui.Q("FileNameTxtField") as TextField;
        fileNameTxtField.value = fileName + numberOfNextScenario;
        csvManager.fileName = fileName + numberOfNextScenario;
    }
}
