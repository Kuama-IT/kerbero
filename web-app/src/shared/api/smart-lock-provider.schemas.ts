import { z } from "zod";

export const SmartLockProviderEnumSchema = z.enum(["nuki"]);
export type SmartLockProviderEnum = z.infer<typeof SmartLockProviderEnumSchema>;
