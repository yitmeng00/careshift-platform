import {
  HubConnectionBuilder,
  HubConnection,
  LogLevel,
} from "@microsoft/signalr";

let connection: HubConnection | null = null;

export const createScheduleHubConnection = (token: string): HubConnection => {
  connection = new HubConnectionBuilder()
    .withUrl("/hubs/schedule", {
      accessTokenFactory: () => token,
    })
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Warning)
    .build();

  return connection;
};

export const getScheduleHubConnection = (): HubConnection | null => {
  return connection;
};

export const startScheduleHub = async (
  token: string,
): Promise<HubConnection> => {
  if (connection?.state === "Connected") return connection;

  const hub = createScheduleHubConnection(token);
  await hub.start();
  return hub;
};

export const stopScheduleHub = async (): Promise<void> => {
  if (connection) {
    await connection.stop();
    connection = null;
  }
};
