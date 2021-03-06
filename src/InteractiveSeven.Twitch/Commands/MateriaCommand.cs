﻿using InteractiveSeven.Core.Data.Items;
using InteractiveSeven.Core.Diagnostics.Memory;
using InteractiveSeven.Core.Model;
using InteractiveSeven.Twitch.Model;
using System.Linq;
using TwitchLib.Client.Interfaces;

namespace InteractiveSeven.Twitch.Commands
{
    public class MateriaCommand : BaseCommand
    {
        private readonly ITwitchClient _twitchClient;
        private readonly IMateriaAccessor _materiaAccessor;

        public MateriaCommand(ITwitchClient twitchClient, IMateriaAccessor materiaAccessor)
            : base(x => x.MateriaCommandWords, x => x.MateriaSettings.Enabled)
        {
            _twitchClient = twitchClient;
            _materiaAccessor = materiaAccessor;
        }

        public override void Execute(in CommandData commandData)
        {
            if (!IsAllowedToUseCommand(commandData.User)) return;

            string materiaIdText = commandData.Arguments.FirstOrDefault();
            if (materiaIdText != null && byte.TryParse(materiaIdText, out byte materiaId) && materiaId < 91)
            {
                _materiaAccessor.AddMateria(materiaId);
                Materia materia = Materia.All.SingleOrDefault(x => x.Value == materiaId);
                string materiaName = materia == null ? "Unknown Materia" : materia.Name;
                _twitchClient.SendMessage(commandData.Channel,
                    $"Materia {materiaName} Added");
            }
        }
        private bool IsAllowedToUseCommand(in ChatUser user)
            => (Settings.MateriaSettings.AllowMod && user.IsMod)
               || user.IsMe || user.IsBroadcaster;
    }
}