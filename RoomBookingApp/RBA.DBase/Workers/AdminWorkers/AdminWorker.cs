using Microsoft.Extensions.Logging;
using RBA.DBase.Managers.Admin;
using RBA.DBase.Managers.DbWorker;
using RBA.Models.Models.RoomBookingModels;
using RBA.Models.Request.Admin.Cancel;
using RBA.Models.Request.Admin.Login;
using RBA.Models.Request.Admin.Logout;
using RBA.Models.Request.Admin.NewRoom;
using RBA.Models.Request.Admin.Reservations;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Workers.AdminWorkers
{
    public class AdminWorker(IAdminDbManager dbWorker, ILogger<AdminWorker> logger) : IAdminManagment
    {
        public async Task<bool> LoginAsync(LoginRequest request)
        {
            logger.LogInformation("AdminWorker: LoginAsync called with UserName: {UserName}", request.UserName);
            var result = await dbWorker.LoginAsync(request);
            logger.LogInformation("AdminWorker: LoginAsync completed with result: {Result}", result);
            return result;
        }

        public async Task<bool> LogoutAsync(LogoutRequest request)
        {
            logger.LogInformation("AdminWorker: LogoutAsync called");
            var result = await dbWorker.LogoutAsync(request);
            logger.LogInformation("AdminWorker: LogoutAsync completed with result: {Result}", result);
            return result;
        }

        public async Task<RoomModel> InsertNewRoom(NewRoomRequest roomRequest)
        {
            logger.LogInformation("AdminWorker: InsertNewRoom called with RoomName: {RoomName}", roomRequest.Name);
            var result = await dbWorker.InsertNewRoom(roomRequest);
            logger.LogInformation("AdminWorker: InsertNewRoom completed with result: {Result}", result);
            return result;
        }

        public async Task<List<AmenityModel>> GetRoomConfigs()
        {
            logger.LogInformation("AdminWorker: GetRoomConfigs called");
            var result = await dbWorker.GetRoomConfigs();
            logger.LogInformation("AdminWorker: GetRoomConfigs completed with result: {Result}", result);
            return result;
        }

        public async Task<int> GetTotalBookings()
        {
            logger.LogInformation("AdminWorker: GetTotalBookings called");
            var result = await dbWorker.GetTotalBookings();
            logger.LogInformation("AdminWorker: GetTotalBookings completed with result: {Result}", result);
            return result;
        }

        public async Task<int> GetTodayBookings()
        {
            logger.LogInformation("AdminWorker: GetTodayBookings called");
            var result = await dbWorker.GetTodayBookings();
            logger.LogInformation("AdminWorker: GetTodayBookings completed with result: {Result}", result);
            return result;
        }

        public async Task<int> GetAllRoomsCount()
        {
            logger.LogInformation("AdminWorker: GetAllRoomsCount called");
            var result = await dbWorker.GetAllRoomsCount();
            logger.LogInformation("AdminWorker: GetAllRoomsCount completed with result: {Result}", result);
            return result;
        }

        public async Task<List<ReservationModel>> GetAllReservations()
        {
            logger.LogInformation("AdminWorker: GetAllReservations called");
            var result = await dbWorker.GetAllReservations();
            logger.LogInformation("AdminWorker: GetAllReservations completed with result: {Result}", result);
            return result;
        }

        public async Task<bool> CancelBookingAsync(CancelBookingRequest request)
        {
            logger.LogInformation("AdminWorker: CancelBookingAsync called");
            var result = await dbWorker.CancelBookingAsync(request);
            logger.LogInformation("AdminWorker: CancelBookingAsync completed with result: {Result}", result);
            return result;
        }
    }
}