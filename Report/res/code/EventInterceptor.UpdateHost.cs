public Task UpdateHostAsync(int eventId, int newHostId)
{
    _eventValidator.ValidateEventExists(eventId);
    _personValidator.ValidatePersonExists(newHostId);
    _eventValidator.CanUpdateHost(eventId);
    return null;
}
