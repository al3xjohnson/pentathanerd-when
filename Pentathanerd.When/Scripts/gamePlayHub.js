(function ($, gameController) {
    // Declare a proxy to reference the hub.
    var gamePlayHub = $.connection.gamePlayHub;

    // 8 = Backspace, 46 = Delete
    var invalidKeys = [8, 46];
    
    // Start the connection.
    $.connection.hub.start()
        .done(function() {
            initializeButtons();
        });

    function initializeButtons() {
        initializeStartGameButton();
        initializeResetGameButton();
        initializeLeftScreenButton();
        initializeRightScreenButton();
    }

    function initializeStartGameButton() {
        var startGameButton = $("#startGameButton");

        startGameButton.click(function () {
            gamePlayHub.server.startGame();
        });
    }

    function initializeResetGameButton() {
        var resetGameButton = $("#resetGameButton");
        resetGameButton.click(function () {
            gamePlayHub.server.resetGame();
        });
    }

    function initializeLeftScreenButton() {
        var leftScreenButton = $("#leftScreenButton");
        leftScreenButton.click(function () {
            registerPlayer("left");
        });
    }

    function initializeRightScreenButton() {
        var rightScreenButton = $("#rightScreenButton");
        rightScreenButton.click(function () {
            registerPlayer("right");
        });
    }

    function registerPlayer(screenLocation) {
        gamePlayHub.server.registerPlayer(screenLocation);
    }
    
    document.onkeydown = function (e) {
        e = e || window.event;
        var charCode = e.which;

        // Prevent backspace from navigating away from the page
        if (charCode === 8) {
            e.preventDefault();
        }
    }

    document.onkeypress = function(e) {
        if (!gameController.isGameActive()) {
            return false;
        }

        e = e || window.event;
        var charCode = e.which;
        var charStr = String.fromCharCode(charCode);

        if ($.inArray(charCode, invalidKeys) === 0)
            return false;

        recordKeyStroke(charStr);
        return true;
    }

    function recordKeyStroke(keyStroke) {
        var correct = gameController.isCorrectKeyStroke(keyStroke);
        var textIterator = gameController.getTextIterator();

        gamePlayHub.server.recordKeyStroke(correct, textIterator);
    }

    gamePlayHub.client.startGame = function(gameTime) {
        gameController.startGame(gameTime);
    }

    gamePlayHub.client.resetGame = function () {
        gameController.resetGame();
    }

    gamePlayHub.client.endGame = function(quitEarly) {
        gameController.endGame(quitEarly);
    }

    gamePlayHub.client.endGameForEarlyWinner = function (remainingTime) {
        gameController.endGameForEarlyWinner(remainingTime);
    }

    gamePlayHub.client.loadChallengeText = function (challengeText) {
        gameController.loadChallengeText(challengeText);
    }

    gamePlayHub.client.setPlayersTurn = function (value) {
        gameController.setPlayersTurn(value);
    }

    gamePlayHub.client.flipArrow = function (direction) {
        gameController.updateTurnIndicator(direction);
    }

    gamePlayHub.client.updateIteratorPosition = function(iterator) {
        gameController.updateTextValues(iterator);
    }

    gamePlayHub.client.updateStats = function (hitPercentage, completionPercentage, score) {
        gameController.updateStats(hitPercentage, completionPercentage, score);
    }

    gamePlayHub.client.flashBackground = function(isActivePlayer) {
        gameController.flashBackground(isActivePlayer);
    }

    gamePlayHub.client.enableStartGameButton = function() {
        gameController.updateStartGameButtonState("enabled");
    }

    gamePlayHub.client.enableResetGameButton = function () {
        gameController.updateResetGameButtonState("enabled");
    }

    gamePlayHub.client.showGameControls = function() {
        gameController.updateGameControlsDisplay("show");
    }

    gamePlayHub.client.setScreenSelected = function (location) {
        gameController.setScreenSelected(location);
    }

    gamePlayHub.client.showPlayerSelectionModal = function() {
        setSelectedScreens();
        gameController.updatePlayerSelectionModal("show");
    }

    function setSelectedScreens() {
        gamePlayHub.server.setSelectedScreens();
    }

    gamePlayHub.client.updateDisplayForLateComer = function (timeRemaining, challengeTextValue, iterator, screenSelected, hitPercentage, completionPercentage, score) {
        gameController.updateDisplayForLateComer(timeRemaining,
            challengeTextValue,
            iterator,
            screenSelected,
            hitPercentage,
            completionPercentage,
            score);
    }

}(window.jQuery, gameController));