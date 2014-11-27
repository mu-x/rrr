using System;
using System.Collections.Generic;

public static partial class Say
{
    /** Sets up Russian translations */
    public static void Russian()
    {
        RRR = "Реальный Русский Рейсинг";
        TOUCH = "... touch to continue ...";
        START = "Поехали!";

        TRACKS = new Dictionary<int, string>()
        {
            { 1, "Лесное кольцо" }, { 2, "Длинная дорога" }
        };

        PRICE = "Цена";
        STEARING = "Манёвры";
        ARMOR = "Броня";
        FRONT = "Передний привод";
        REAR = "Задний привод";

        COUNT = new[] { "Поехали!", "Внимание", "На Старт" };

        RESUME = "Вернуться в игру";
        RESTART = "Перезапустить";
        MENU = "Главное меню";

        RM_RF = "Свободная езда";
        RM_CC = "Езда по точкам {0} круг(ов)";
        RM_AI = "Реальные гонки {0} круг(ов) {1} соперник(ов)";

        TIME = "Время заезда";
        LAPS = "Круг {0} из {1} всего";
        CHECK = "Точек проехано: {0}";
        PLACE = "Позиция {0} из {1}";
    }
}

